using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace CGame.Editor
{
    public class CreateElementsCommand : CommandBase
    {
        private readonly GraphView _graphView;
        private readonly List<GraphElement> _elements;

        public CreateElementsCommand(GraphView graphView, IEnumerable<GraphElement> elements)
        {
            _graphView = graphView;
            _elements = new List<GraphElement>(elements);
        }
        
        public override void Execute()
        {
            foreach (var graphElement in _elements)
            {
                _graphView.AddElement(graphElement);
            }
        }

        public override void Undo()
        {
            foreach (var graphElement in _elements)
            {
                _graphView.RemoveElement(graphElement);
            }
        }
    }
}