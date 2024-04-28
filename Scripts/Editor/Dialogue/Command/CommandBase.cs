namespace CGame.Editor
{
    public abstract class CommandBase
    {
        public abstract void Execute();
        public abstract void Undo();
    }
}