using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Syncfusion.UI.Xaml.TreeView.Engine;
using Syncfusion.Windows.Shared;
using WolvenKit.Common;
using WolvenKit.Common.FNV1A;
using WolvenKit.Functionality.Controllers;
using WolvenKit.RED4.Archive;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Archive.CR2W;
using WolvenKit.RED4.Types;
using WolvenKit.ViewModels.Shell;

namespace WolvenKit.ViewModels.Documents
{
    public interface INodeViewModel
    {
        public System.Windows.Point Location { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }

    public class CNameWrapper : ReactiveObject, INodeViewModel
    {
        public CName CName => Socket.File;

        [Reactive] public System.Windows.Point Location { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        [Reactive] public ReferenceSocket Socket { get; set; }

        public List<ReferenceSocket> SelfSocket => new List<ReferenceSocket>(new ReferenceSocket[] { Socket });

        public RDTDataViewModel DataViewModel { get; set; }

        public CNameWrapper(RDTDataViewModel vm, ReferenceSocket socket)
        {
            DataViewModel = vm;
            Socket = socket;
            OpenRefCommand = new DelegateCommand(_ => ExecuteOpenRef(), _ => CanOpenRef());
            LoadRefCommand = new DelegateCommand(_ => ExecuteLoadRef(), _ => CanLoadRef());
        }

        public ICommand OpenRefCommand { get; private set; }
        private bool CanOpenRef() => CName != null;
        private void ExecuteOpenRef()
        {
            Locator.Current.GetService<AppViewModel>().OpenFileFromDepotPath(CName);
        }

        public ICommand LoadRefCommand { get; private set; }
        private bool CanLoadRef() => CName != null && DataViewModel.File.RelativePath != CName;
        private void ExecuteLoadRef()
        {
            var cr2w = DataViewModel.File.GetFileFromDepotPathOrCache(CName);
            if (cr2w != null && cr2w.RootChunk != null)
            {
                var chunk = new ChunkViewModel(cr2w.RootChunk, Socket)
                {
                    Location = Location
                };
                DataViewModel.Nodes.Remove(this);
                DataViewModel.Nodes.Add(chunk);
                DataViewModel.LookForReferences(chunk);
            }
        }
    }

    public class RDTDataViewModel : RedDocumentTabViewModel, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new();

        protected readonly IRedType _data;

        public RDTDataViewModel(IRedType data, RedDocumentViewModel file)
        {
            File = file;
            _data = data;
            Header = _data.GetType().Name;

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                OnDemandLoadingCommand = new DelegateCommand<TreeViewNode>(ExecuteOnDemandLoading, CanExecuteOnDemandLoading);
                OpenImportCommand = new Functionality.Commands.DelegateCommand<ICR2WImport>(ExecuteOpenImport);
                if (SelectedChunk == null)
                {
                    SelectedChunk = Chunks[0];
                }
                //ExportChunkCommand = new DelegateCommand<ChunkViewModel>((p) => ExecuteExportChunk(p), (p) => CanExportChunk(p));

                //this.HandleActivation()
            });


            //if (!Nodes.Contains(Chunks[0]))
            //{
            //    Nodes.Add(Chunks[0]);
            //    _nodePaths.Add(Chunks[0].RelativePath);
            //}
            //LookForReferences(Chunks[0]);


            Nodes.Add(new CNameWrapper(this, new ReferenceSocket(Chunks[0].RelativePath)));
            _nodePaths.Add(Chunks[0].RelativePath);

            //var refCopy = References.ToList();

            //foreach (var reference in refCopy)
            //{
            //    if (Nodes.Where(x => x is ChunkViewModel cvm && cvm.RelativePath == reference.Input.File).Count() == 0)
            //    {
            //        var cr2w = File.GetFileFromDepotPathOrCache(reference.Input.File);
            //        if (cr2w != null && cr2w.RootChunk != null)
            //        {
            //            var chunk = new ChunkViewModel(cr2w.RootChunk, reference.Input);
            //            Nodes.Add(chunk);
            //            LookForReferences(chunk, cr2w.RootChunk);
            //        }
            //    }
            //}

            //refCopy = References.ToList();

            //foreach (var reference in refCopy)
            //{
            //    if (Nodes.Where(x => x is ChunkViewModel cvm && cvm.RelativePath == reference.Input.File).Count() == 0)
            //    {
            //        var cr2w = File.GetFileFromDepotPathOrCache(reference.Input.File);
            //        if (cr2w != null && cr2w.RootChunk != null)
            //        {
            //            var chunk = new ChunkViewModel(cr2w.RootChunk, reference.Input);
            //            Nodes.Add(chunk);
            //            LookForReferences(chunk, cr2w.RootChunk);
            //        }
            //    }
            //}

            //RootChunk = Chunks[0];

            //if (Chunks != null)
            //{
            //    foreach (ChunkViewModel item in Chunks)
            //    {
            //        item.WhenAnyValue(x => x.IsDirty).Subscribe(x => IsDirty |= x);
            //    }
            //}
            //_file.WhenAnyValue(x => x).Subscribe(x => IsDirty |= true);
        }

        public RDTDataViewModel(string header, IRedType data, RedDocumentViewModel file) : this(data, file)
        {
            Header = header;
        }

        #region properties

        public override ERedDocumentItemType DocumentItemType => ERedDocumentItemType.MainFile;

        //[Reactive] public ObservableCollection<ChunkPropertyViewModel> ChunkProperties { get; set; } = new();

        //public IReadOnlyList<IRedRef> Imports => _file is CR2WFile cr2w ? cr2w.Imports : new ReadOnlyCollection<IRedRef>(new List<IRedRef>());

        //public List<ICR2WBuffer> Buffers => _file.Buffers;

        private List<ChunkViewModel> _chunks;

        public List<ChunkViewModel> Chunks
        {
            get
            {
                if (_chunks == null || _chunks.Count == 0)
                {
                    _chunks = new List<ChunkViewModel>
                    {
                        GenerateChunks()
                    };
                    //SelectedChunk = _chunks[0];
                }
                return _chunks;
            }
            set => _chunks = value;
        }

        public virtual ChunkViewModel GenerateChunks() => new ChunkViewModel(_data, this);

        [Reactive] public ChunkViewModel SelectedChunk { get; set; }

        [Reactive] public ChunkViewModel RootChunk { get; set; }

        [Reactive] public ObservableCollection<object> Nodes { get; set; } = new();

        [Reactive] public ObservableCollection<RedReference> References { get; set; } = new();

        [Reactive] public IRedRef SelectedImport { get; set; }

        public delegate void LayoutNodesDelegate();

        public LayoutNodesDelegate LayoutNodes;

        private List<CName> _nodePaths = new();

        #endregion

        #region commands

        public ICommand OnDemandLoadingCommand { get; private set; }

        private bool CanExecuteOnDemandLoading(TreeViewNode node)
        {
            if (node.Content is GroupedChunkViewModel)
            {
                return true;
            }

            if (node.Content is ChunkViewModel cvm && cvm.HasChildren())
            {
                return true;
            }

            return false;
        }

        private void ExecuteOnDemandLoading(TreeViewNode node)
        {
            if (node.ChildNodes.Count > 0)
            {
                node.IsExpanded = true;
                return;
            }

            node.ShowExpanderAnimation = true;

            if (node.Content is GroupedChunkViewModel gcvm)
            {
                Application.Current.MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        node.PopulateChildNodes(gcvm.TVProperties);
                        if (gcvm.TVProperties.Count > 0)
                        {
                            node.IsExpanded = true;
                        }

                        node.ShowExpanderAnimation = false;
                    }));
            }

            if (node.Content is ChunkViewModel cvm)
            {
                Application.Current.MainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        cvm.CalculateProperties();
                        node.PopulateChildNodes(cvm.TVProperties);
                        if (cvm.TVProperties.Count > 0)
                        {
                            node.IsExpanded = true;
                        }

                        node.ShowExpanderAnimation = false;
                    }));
            }
        }

        public ICommand OpenImportCommand { get; private set; }
        private void ExecuteOpenImport(ICR2WImport input)
        {
            var depotpath = input.DepotPath;
            var key = FNV1A64HashAlgorithm.HashString(depotpath);

            var _gameControllerFactory = Locator.Current.GetService<IGameControllerFactory>();
            var _archiveManager = Locator.Current.GetService<IArchiveManager>();

            if (_archiveManager.Lookup(key).HasValue)
            {
                _gameControllerFactory.GetController().AddToMod(key);
            }
        }

        //public ICommand ExportChunkCommand { get; private set; }
        //private bool CanExportChunk(ChunkViewModel cvm) => cvm.Properties.Count > 0;
        //private void ExecuteExportChunk(ChunkViewModel cvm)
        //{
        //    Stream myStream;
        //    var saveFileDialog = new SaveFileDialog();

        //    saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
        //    saveFileDialog.FilterIndex = 2;
        //    saveFileDialog.FileName = cvm.Type + ".json";
        //    saveFileDialog.RestoreDirectory = true;

        //    if (saveFileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        if ((myStream = saveFileDialog.OpenFile()) != null)
        //        {
        //            var dto = new RedClassDto(cvm.Data);
        //            var json = JsonConvert.SerializeObject(dto, Formatting.Indented);

        //            if (string.IsNullOrEmpty(json))
        //            {
        //                throw new SerializationException();
        //            }

        //            myStream.Write(json.ToCharArray().Select(c => (byte)c).ToArray());
        //            myStream.Close();
        //        }
        //    }
        //}

        #endregion

        #region methods

        public void LookForReferences(ChunkViewModel cvm)
        {
            LookForReferences(cvm, (RedBaseClass)cvm.Data, "root");

            foreach (var reference in References)
            {
                if (!_nodePaths.Contains(reference.Input.File))
                {
                    if (reference.Input.File != null)
                    {
                        Nodes.Add(new CNameWrapper(this, reference.Input));
                        _nodePaths.Add(reference.Input.File);
                    }
                }
            }

            if (LayoutNodes != null)
            {
                LayoutNodes();
            }
        }

        public void LookForReferences(ChunkViewModel cvm, RedBaseClass data, string xpath)
        {
            foreach (var pi in RedReflection.GetTypeInfo(data.GetType()).PropertyInfos)
            {
                if (pi.Type.IsAssignableTo(typeof(IRedRef)))
                {
                    var res = (IRedRef)data.GetProperty(pi.RedName);
                    var sourceSocket = new ReferenceSocket(cvm.RelativePath, xpath + "." + pi.RedName);
                    cvm.References.Add(sourceSocket);

                    if (res != null && !string.IsNullOrEmpty(res.DepotPath))
                    {
                        sourceSocket.IsConnected = true;
                        ReferenceSocket destSocket = null;
                        foreach (var reference in References)
                        {
                            if (reference.Input.File == res.DepotPath)
                            {
                                destSocket = reference.Input;
                            }
                        }
                        if (destSocket == null)
                        {
                            destSocket = new ReferenceSocket(res.DepotPath)
                            {
                                IsConnected = true
                            };
                        }
                        References.Add(new RedReference(this, sourceSocket, destSocket));
                    }
                }
                else
                {
                    var name = !string.IsNullOrEmpty(pi.RedName) ? pi.RedName : pi.Name;
                    var prop = data.GetProperty(name);
                    if (prop is RedBaseClass rbc)
                    {
                        LookForReferences(cvm, rbc, xpath + "." + pi.RedName);
                    }
                    else if (prop is IRedHandle irh)
                    {
                        if (irh.GetValue() is RedBaseClass rbc2)
                        {
                            LookForReferences(cvm, rbc2, xpath + "." + pi.RedName);
                        }
                    }
                    else if (prop is IRedArray ira)
                    {
                        var i = 0;
                        foreach (var item in ira)
                        {
                            if (item is RedBaseClass rbc3)
                            {
                                LookForReferences(cvm, rbc3, xpath + "." + pi.RedName + $"[{i}]");
                            }
                            else if (item is IRedHandle irh2)
                            {
                                if (irh2.GetValue() is RedBaseClass rbc4)
                                {
                                    LookForReferences(cvm, rbc4, xpath + "." + pi.RedName + $"[{i}]");
                                }
                            }
                            i++;
                        }
                    }
                    else if (prop is SerializationDeferredDataBuffer sddb && sddb.Data is Package04 p4)
                    {
                        var i = 0;
                        foreach (var item in p4.Chunks)
                        {
                            if (item is RedBaseClass rbc5)
                            {
                                LookForReferences(cvm, rbc5, xpath + "." + pi.RedName + $"[{i}]");
                            }
                            i++;
                        }
                    }
                    else if (prop is SharedDataBuffer sdb)
                    {
                        if (sdb.Data is Package04 p42)
                        {
                            var i = 0;
                            foreach (var item in p42.Chunks)
                            {
                                if (item is RedBaseClass rbc5)
                                {
                                    LookForReferences(cvm, rbc5, xpath + "." + pi.RedName + $"[{i}]");
                                }
                                i++;
                            }
                        }
                    }
                    else if (prop is DataBuffer db)
                    {
                        if (db.Data is Package04 p42)
                        {
                            var i = 0;
                            foreach (var item in p42.Chunks)
                            {
                                if (item is RedBaseClass rbc5)
                                {
                                    LookForReferences(cvm, rbc5, xpath + "." + pi.RedName + $"[{i}]");
                                }
                                i++;
                            }
                        }
                        else if (db.Data is CR2WList cl)
                        {
                            var i = 0;
                            foreach (var item in cl.Files)
                            {
                                if (item.RootChunk is RedBaseClass rbc5)
                                {
                                    LookForReferences(cvm, rbc5, xpath + "." + pi.RedName + $"[{i}]");
                                }
                                i++;
                            }
                        }
                    }
                }
            }


        }

        //public Red4File GetFile() => _file;

        #endregion
    }

    public class RedReference : ReactiveObject
    {
        [Reactive] public RDTDataViewModel Graph { get; set; }

        [Reactive] public ReferenceSocket Input { get; set; }

        [Reactive] public ReferenceSocket Output { get; set; }

        public RedReference(RDTDataViewModel graph, ReferenceSocket output, ReferenceSocket input)
        {
            Graph = graph;
            Output = output;
            Input = input;
        }

        //public ConnectionViewModel(RDTGraphViewModel graph, graphGraphConnectionDefinition connection)
        //{
        //    Graph = graph;
        //    Input = Graph.SocketLookup[connection.Source.Chunk.GetHashCode()];
        //    Input.Connections.Add(this);
        //    Output = Graph.SocketLookup[connection.Destination.Chunk.GetHashCode()];
        //    Output.Connections.Add(this);
        //}
    }
}
