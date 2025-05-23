using System;
using System.Collections.Generic;
using System.Linq;

namespace DreadZitoEngine.Runtime.Gameplay.Players
{
    [Serializable]
    public class PlayerMoveBlocker
    {
        [Serializable]
        public class MovementBlocker
        {
            public string SourceId { get; }
            public bool ShouldBlock { get; }

            public MovementBlocker(string sourceId, bool shouldBlock)
            {
                SourceId = sourceId;
                ShouldBlock = shouldBlock;
            }
        }
        
        private Dictionary<int, List<MovementBlocker>> blockersByPriority = new Dictionary<int, List<MovementBlocker>>();
        public List<MovementBlocker>[] Blockers => blockersByPriority.Values.ToArray();
        
        public void AddBlocker(string sourceId, int priority, bool shouldBlock)
        {
            RemoveBlocker(sourceId); // Remove existing blockers from the same source

            if (!blockersByPriority.ContainsKey(priority))
            {
                blockersByPriority[priority] = new List<MovementBlocker>();
            }
            blockersByPriority[priority].Add(new MovementBlocker(sourceId, shouldBlock));
        }

        public void RemoveBlocker(string sourceId)
        {
            foreach (var priorityGroup in blockersByPriority.Values)
            {
                for (int i = priorityGroup.Count - 1; i >= 0; i--)
                {
                    if (priorityGroup[i].SourceId == sourceId)
                    {
                        priorityGroup.RemoveAt(i);
                    }
                }
            }
        }

        public bool ShouldBlockMovement()
        {
            var sortedPriorities = blockersByPriority.Keys.OrderByDescending(p => p).ToList();

            foreach (var priority in sortedPriorities)
            {
                var blockers = blockersByPriority[priority];
                if (blockers.Count == 0) continue;

                // Block if any blocker in this priority group is blocking
                if (blockers.Any(b => b.ShouldBlock))
                    return true;
            }

            return false; // No blockers active
        }

        public override string ToString()
        {
            var str = "";
            foreach (var blockerList in Blockers)
            {
                str += $"Blocker---------:\n";
                foreach (var blocker in blockerList)
                {
                    str += $"{blocker.SourceId} - {blocker.ShouldBlock}\n";
                }
            }
            
            return str;
        }
    }
}