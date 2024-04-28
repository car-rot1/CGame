using System.Collections;
using System.Collections.Generic;

namespace CGame
{
    public class QuestSystem : SingletonClass<QuestSystem>, IEnumerable<KeyValuePair<string, QuestData>>
    {
        private Dictionary<string, QuestData> _questDataDic;

        protected override void Init()
        {
            _questDataDic = new Dictionary<string, QuestData>();
        }

        public void AddQuest(QuestData questData) => _questDataDic.Add(questData.Id, questData);

        public QuestData GetQuest(string id) => _questDataDic[id];
        public IEnumerable<QuestData> GetAllQuest() => _questDataDic.Values;
        
        public QuestData this[string key] => GetQuest(key);
        public IEnumerator<KeyValuePair<string, QuestData>> GetEnumerator() => _questDataDic.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}