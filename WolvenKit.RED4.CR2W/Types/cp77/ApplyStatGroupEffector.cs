using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class ApplyStatGroupEffector : gameEffector
	{
		private gameStatsObjectID _target;
		private TweakDBID _record;
		private CString _applicationTarget;
		private CUInt64 _modGroupID;

		[Ordinal(0)] 
		[RED("target")] 
		public gameStatsObjectID Target
		{
			get => GetProperty(ref _target);
			set => SetProperty(ref _target, value);
		}

		[Ordinal(1)] 
		[RED("record")] 
		public TweakDBID Record
		{
			get => GetProperty(ref _record);
			set => SetProperty(ref _record, value);
		}

		[Ordinal(2)] 
		[RED("applicationTarget")] 
		public CString ApplicationTarget
		{
			get => GetProperty(ref _applicationTarget);
			set => SetProperty(ref _applicationTarget, value);
		}

		[Ordinal(3)] 
		[RED("modGroupID")] 
		public CUInt64 ModGroupID
		{
			get => GetProperty(ref _modGroupID);
			set => SetProperty(ref _modGroupID, value);
		}

		public ApplyStatGroupEffector(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
