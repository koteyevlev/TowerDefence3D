using System.Collections.Generic;

namespace TowerDefence3d.Scripts.Setup
{
    public class GameBehaviourCollection
    {
        private List<GameBehaviour> _behaviours = new List<GameBehaviour>();

        public bool IsEmpty => _behaviours.Count == 0;

        public void Add(GameBehaviour behaviour)
        {
            _behaviours.Add(behaviour);
        }

        public void GameUpdate()
        {
            for (int i = 0; i < _behaviours.Count; i++)
            {
                if (!_behaviours[i].GameUpdate())
                {
                    int lastIndex = _behaviours.Count - 1;
                    _behaviours[i] = _behaviours[lastIndex];
                    _behaviours.RemoveAt(lastIndex);
                    i -= 1;
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _behaviours.Count; i++)
            {
                _behaviours[i].Recycle();
            }
            _behaviours.Clear();
        }
    }
}
