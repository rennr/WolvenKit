using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class AirHoverDecisions : LocomotionAirDecisions
	{
		private wCHandle<gameObject> _executionOwner;
		private CHandle<DefaultTransitionStatusEffectListener> _statusEffectListener;
		private CBool _hasStatusEffect;

		[Ordinal(3)] 
		[RED("executionOwner")] 
		public wCHandle<gameObject> ExecutionOwner
		{
			get => GetProperty(ref _executionOwner);
			set => SetProperty(ref _executionOwner, value);
		}

		[Ordinal(4)] 
		[RED("statusEffectListener")] 
		public CHandle<DefaultTransitionStatusEffectListener> StatusEffectListener
		{
			get => GetProperty(ref _statusEffectListener);
			set => SetProperty(ref _statusEffectListener, value);
		}

		[Ordinal(5)] 
		[RED("hasStatusEffect")] 
		public CBool HasStatusEffect
		{
			get => GetProperty(ref _hasStatusEffect);
			set => SetProperty(ref _hasStatusEffect, value);
		}

		public AirHoverDecisions(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
