using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Facebook.Yoga
{
    public enum LayoutType
    {
        kLayout = 0,
        kMeasure = 1,
        kCachedLayout = 2,
        kCachedMeasure = 3
    }

    public enum LayoutPassReason
    {
        kInitial = 0,
        kAbsLayout = 1,
        kStretch = 2,
        kMultilineStretch = 3,
        kFlexLayout = 4,
        kMeasureChild = 5,
        kAbsMeasureChild = 6,
        kFlexMeasure = 7,
        kGridLayout = 8,
        COUNT
    }

    public enum YGMeasureMode
    {
        kYGMeasureModeUndefined,
        kYGMeasureModeExactly,
        kYGMeasureModeAtMost
    }

    public struct LayoutData
    {
        public int Layouts;
        public int Measures;
        public uint MaxMeasureCache;
        public int CachedLayouts;
        public int CachedMeasures;
        public int MeasureCallbacks;
        public int[] MeasureCallbackReasonsCount;

        public LayoutData()
        {
            MeasureCallbackReasonsCount = new int[(int)LayoutPassReason.COUNT];
        }
    }

    public static class LayoutPassReasonHelper
    {
        public static string ToString(LayoutPassReason value)
        {
            return value switch
            {
                LayoutPassReason.kInitial => "initial",
                LayoutPassReason.kAbsLayout => "abs_layout",
                LayoutPassReason.kStretch => "stretch",
                LayoutPassReason.kMultilineStretch => "multiline_stretch",
                LayoutPassReason.kFlexLayout => "flex_layout",
                LayoutPassReason.kMeasureChild => "measure",
                LayoutPassReason.kAbsMeasureChild => "abs_measure",
                LayoutPassReason.kFlexMeasure => "flex_measure",
                LayoutPassReason.kGridLayout => "grid_layout",
                _ => "unknown"
            };
        }
    }

    public enum EventType
    {
        NodeAllocation,
        NodeDeallocation,
        NodeLayout,
        LayoutPassStart,
        LayoutPassEnd,
        MeasureCallbackStart,
        MeasureCallbackEnd,
        NodeBaselineStart,
        NodeBaselineEnd
    }

    public class Event
    {
        public class Data
        {
            private readonly object? _data;
            private readonly EventType _eventType;

            internal Data(object? data, EventType eventType)
            {
                _data = data;
                _eventType = eventType;
            }

            public TypedData<T> Get<T>() where T : EventTypedDataBase, new()
            {
                if (_data is TypedData<T> typedData)
                {
                    return typedData;
                }
                return new TypedData<T>();
            }
        }

        public class TypedData<T> : EventTypedDataBase where T : EventTypedDataBase, new()
        {
            public T Data { get; } = new T();
        }

        public abstract class EventTypedDataBase { }

        public class NodeAllocationData : EventTypedDataBase
        {
            public Config? Config { get; set; }
        }

        public class NodeDeallocationData : EventTypedDataBase
        {
            public Config? Config { get; set; }
        }

        public class LayoutPassEndData : EventTypedDataBase
        {
            public LayoutData? LayoutData { get; set; }
        }

        public class MeasureCallbackEndData : EventTypedDataBase
        {
            public float Width { get; set; }
            public YGMeasureMode WidthMeasureMode { get; set; }
            public float Height { get; set; }
            public YGMeasureMode HeightMeasureMode { get; set; }
            public float MeasuredWidth { get; set; }
            public float MeasuredHeight { get; set; }
            public LayoutPassReason Reason { get; set; }
        }

        public class NodeLayoutData : EventTypedDataBase
        {
            public LayoutType LayoutType { get; set; }
        }

        public delegate void Subscriber(Node? node, EventType eventType, Data eventData);

        private sealed class SubscriberNode
        {
            public Subscriber? Subscriber { get; }
            public SubscriberNode? Next { get; }

            public SubscriberNode(Subscriber? subscriber)
            {
                Subscriber = subscriber;
            }
        }

        private static readonly LinkedList<SubscriberNode> _subscribers = new();
        private static readonly object _lock = new();

        public static void Reset()
        {
            lock (_lock)
            {
                _subscribers.Clear();
            }
        }

        public static void Subscribe(Subscriber? subscriber)
        {
            if (subscriber == null)
                return;

            lock (_lock)
            {
                _subscribers.AddLast(new SubscriberNode(subscriber));
            }
        }

        public static void Publish<T>(Node? node, in T eventData = default) where T : EventTypedDataBase, new()
        {
            var data = new Data(eventData, GetEventType<T>());
            PublishCore(node, GetEventType<T>(), data);
        }

        public static void Publish(Node? node, EventType eventType, in Data eventData)
        {
            PublishCore(node, eventType, eventData);
        }

        private static void PublishCore(Node? node, EventType eventType, Data eventData)
        {
            List<Subscriber>? subscribersCopy;
            lock (_lock)
            {
                subscribersCopy = _subscribers.Select(n => n.Subscriber).ToList();
            }

            foreach (var subscriber in subscribersCopy)
            {
                try
                {
                    subscriber?.Invoke(node, eventType, eventData);
                }
                catch
                {
                    // Swallow exceptions to match C++ behavior
                }
            }
        }

        private static EventType GetEventType<T>() where T : EventTypedDataBase, new()
        {
            if (typeof(T) == typeof(NodeAllocationData))
                return EventType.NodeAllocation;
            if (typeof(T) == typeof(NodeDeallocationData))
                return EventType.NodeDeallocation;
            if (typeof(T) == typeof(LayoutPassEndData))
                return EventType.LayoutPassEnd;
            if (typeof(T) == typeof(MeasureCallbackEndData))
                return EventType.MeasureCallbackEnd;
            if (typeof(T) == typeof(NodeLayoutData))
                return EventType.NodeLayout;
            
            return EventType.NodeAllocation; // Default
        }
    }
}

