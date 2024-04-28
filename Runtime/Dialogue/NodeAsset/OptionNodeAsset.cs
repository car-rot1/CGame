namespace CGame
{
    public sealed class OptionNodeAsset : NodeAssetBase
    {
        public string option;
        public override bool Finish { get; protected set; } = true;
        public override void Execute() { }
    }
}