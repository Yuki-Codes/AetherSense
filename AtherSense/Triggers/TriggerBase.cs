using AetherSense.Patterns;

namespace AetherSense.Triggers
{
	public abstract class TriggerBase
	{
		public PatternBase Pattern { get; set; }

		public abstract void Initialize();
		public abstract void OnEditorGui();
	}
}
