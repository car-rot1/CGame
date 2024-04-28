using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace CGame.Editor
{
    public class MoveElementCommand : CommandBase
    {
        private readonly List<GraphElement> _elements;
        private readonly Vector2 _movementDelta;
        
        public MoveElementCommand(IEnumerable<GraphElement> elements, Vector2 movementDelta)
        {
            _elements = new List<GraphElement>(elements);
            _movementDelta = movementDelta;
        }
        
        public override void Execute()
        {
            foreach (var graphElement in _elements)
            {
                var position = graphElement.GetPosition();
                position.position += _movementDelta;
                graphElement.SetPosition(position);
            }
        }

        public override void Undo()
        {
            foreach (var graphElement in _elements)
            {
                var position = graphElement.GetPosition();
                position.position -= _movementDelta;
                graphElement.SetPosition(position);
            }
        }
    }
}