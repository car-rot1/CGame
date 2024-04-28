using System;

namespace CGame
{
    [Serializable]
    public class QuestData
    {
        public string Id { get; }
        public string Content { get; }
        public string ProcessContent => _getProcessContent?.Invoke();
        private Func<string> _getProcessContent;
        
        private bool _isFinish;
        public bool IsFinish
        {
            get => _isFinish;
            set
            {
                if (_isFinish == value)
                    return;
                OnFinishChange?.Invoke((_isFinish, value));
                _isFinish = value;
            }
        }
        public event Action<(bool oldValue, bool newValue)> OnFinishChange;

        public QuestData(string id, string content, Func<string> getProcessContent)
        {
            Id = id;
            Content = content;
            _getProcessContent = getProcessContent;
        }
    }
}