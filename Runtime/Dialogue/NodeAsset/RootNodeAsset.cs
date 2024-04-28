namespace CGame
{
    public sealed class RootNodeAsset : NodeAssetBase
    {
        public override bool Finish { get; protected set; } = true;

        public override void Execute() {  }
    }
}