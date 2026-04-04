using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Facebook.Yoga
{
    public class LayoutableChildren<T> : IEnumerable<T> where T : Node
    {
        private readonly T _node;

        public LayoutableChildren(T node)
        {
            _node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public Iterator GetEnumerator()
        {
            return new Iterator(_node);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Iterator : IEnumerator<T>
        {
            private T _node;
            private int _childIndex;
            private LinkedList<(T node, int childIndex)> _backtrack;
            private T _current;

            internal Iterator(T node)
            {
                _node = node;
                _childIndex = -1;
                _backtrack = new LinkedList<(T, int)>();
                _current = null;

                if (node != null && (int)node.GetChildCount() > 0)
                {
                    _childIndex = 0;
                    var firstChild = _node.GetChild(0);
                    if (firstChild.Style.Display == Display.Contents)
                    {
                        SkipContentsNodes();
                    }
                    UpdateCurrent();
                }
            }

            public T Current => _current;
            object IEnumerator.Current => _current;

            public bool MoveNext()
            {
                if (_node == null)
                    return false;

                Next();
                UpdateCurrent();
                return _node != null;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public void Dispose()
            {
            }

            private void UpdateCurrent()
            {
                if (_node != null && _childIndex >= 0 && _childIndex < (int)_node.GetChildCount())
                {
                    _current = (T)(object)_node.GetChild((nuint)_childIndex);
                }
                else
                {
                    _current = null;
                }
            }

            private void Next()
            {
                if (_childIndex + 1 >= (int)_node.GetChildCount())
                {
                    if (_backtrack.Count == 0)
                    {
                        _node = null;
                        _childIndex = 0;
                    }
                    else
                    {
                        var back = _backtrack.First.Value;
                        _backtrack.RemoveFirst();
                        _node = back.node;
                        _childIndex = back.childIndex;
                        Next();
                    }
                }
                else
                {
                    _childIndex++;
                    var child = _node.GetChild((nuint)_childIndex);
                    if (child.Style.Display == Display.Contents)
                    {
                        SkipContentsNodes();
                    }
                }
            }

            private void SkipContentsNodes()
            {
                var currentNode = _node.GetChild((nuint)_childIndex);
                while (currentNode.Style.Display == Display.Contents && currentNode.GetChildCount() > 0)
                {
                    _backtrack.AddFirst((_node, _childIndex));
                    _node = (T)(object)currentNode;
                    _childIndex = 0;
                    currentNode = currentNode.GetChild((nuint)_childIndex);
                }

                if (currentNode.Style.Display == Display.Contents)
                {
                    Next();
                }
            }
        }
    }
}

