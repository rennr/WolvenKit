using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WolvenKit.Common.Extensions;
using WolvenKit.Common.FNV1A;
using WolvenKit.Common.Oodle;
using WolvenKit.Interfaces.Extensions;

namespace WolvenKit.Common.Services
{
    public class HashService : IHashService
    {
        #region Fields

        private const string s_used = "WolvenKit.Common.Resources.usedhashes.kark";
        private const string s_unused = "WolvenKit.Common.Resources.unusedhashes.kark";
        private const string s_userHashes = "user_hashes.txt";

        private readonly Dictionary<ulong, string> _hashes = new();
        private readonly Dictionary<ulong, string> _additionalhashes = new();
        private readonly Dictionary<ulong, string> _userHashes = new();

        #endregion Fields

        #region Constructors

        public HashService()
        {
            Load();
        }

        #endregion Constructors

        #region Methods

        private bool IsAdditionalLoaded() => _additionalhashes.Count > 0;

        public IEnumerable<ulong> GetAllHashes()
        {
            // load additional
            LoadAdditional();
            return _hashes.Keys.Concat(_userHashes.Keys).Concat(_additionalhashes.Keys);
        }

        public bool Contains(ulong key) => _hashes.ContainsKey(key) || _userHashes.ContainsKey(key);

        public string Get(ulong key)
        {
            if (_hashes.ContainsKey(key))
            {
                return _hashes[key];
            }
            if (_userHashes.ContainsKey(key))
            {
                return _userHashes[key];
            }

            // load additional
            LoadAdditional();
            if (_additionalhashes.ContainsKey(key))
            {
                return _additionalhashes[key];
            }

            return "";
        }

        public void Add(string path)
        {
            var hash = FNV1A64HashAlgorithm.HashString(path);
            if (!Contains(hash))
            {
                _userHashes.Add(hash, path);
            }
        }




        private void LoadAdditional()
        {
            if (IsAdditionalLoaded())
            {
                return;
            }

            LoadEmbeddedHashes(s_unused, _additionalhashes);
        }

        private void Load()
        {
            LoadEmbeddedHashes(s_used, _hashes);

            // user hashes
            var assemblyPath = Path.GetDirectoryName(System.AppContext.BaseDirectory);
            var userHashesPath = Path.Combine(assemblyPath ?? throw new InvalidOperationException(), s_userHashes);
            if (File.Exists(userHashesPath))
            {
                using var userFs = new FileStream(userHashesPath, FileMode.Open, FileAccess.Read);
                ReadHashes(userFs, _userHashes);
            }
        }

        private void LoadEmbeddedHashes(string resourceName, Dictionary<ulong, string> hashDictionary)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            // read KARK header
            var oodleCompression = stream.ReadStruct<uint>();
            if (oodleCompression != OodleHelper.KARK)
            {
                throw new DecompressionException($"Incorrect hash file.");
            }

            var outputsize = stream.ReadStruct<uint>();

            // read the rest of the stream
            var outputbuffer = new byte[outputsize];

            var inbuffer = stream.ToByteArray(true);

            OozNative.Kraken_Decompress(inbuffer, inbuffer.Length, outputbuffer, outputbuffer.Length);

            hashDictionary.EnsureCapacity(1_100_000);

            using var ms = new MemoryStream(outputbuffer);
            ReadHashes(ms, hashDictionary);
        }

        private void ReadHashes(Stream memoryStream, IDictionary<ulong, string> hashDict)
        {
            using var sr = new StreamReader(memoryStream);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var hash = FNV1A64HashAlgorithm.HashString(line);
                if (_hashes.ContainsKey(hash))
                {
                    continue;
                }
                if (_userHashes.ContainsKey(hash))
                {
                    continue;
                }
                if (_additionalhashes.ContainsKey(hash))
                {
                    continue;
                }

                hashDict.Add(hash, line);
            }
        }

        #endregion Methods
    }
}
