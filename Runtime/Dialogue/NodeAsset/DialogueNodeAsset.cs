namespace CGame
{
    public sealed class DialogueNodeAsset : NodeAssetBase
    {
        public string author;
        public string content;

        public override bool Finish { get; protected set; }

        public override void Execute()
        {
            UITest.Instance.ShowContent(author, content, () => Finish = true);
        }
    }
}
