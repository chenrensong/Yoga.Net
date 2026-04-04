using System;
using System.Collections.Generic;

namespace Yoga
{
    [Flags]
    public enum Errata
    {
        None = 0,
        Classic = 1 << 0,
        StretchFlexBasis = 1 << 1,
        All = Classic | StretchFlexBasis
    }

    public enum ExperimentalFeature
    {
        WebFlexBaselines
    }

    public enum LogLevel
    {
        Error,
        Warn,
        Info,
        Debug,
        Verbose
    }

    public enum FlexDirection
    {
        Column,
        ColumnReverse,
        Row,
        RowReverse
    }

    public enum PositionType
    {
        Static,
        Relative,
        Absolute
    }

    public delegate void YGLogger(
        Config config,
        Node node,
        LogLevel level,
        string format,
        params object[] args);

    public delegate Node YGCloneNodeFunc(
        Node oldNode,
        Node owner,
        int childIndex);

    public class Node
    {
    }

    public class Config : IDisposable
    {
        private static readonly Lazy<Config> _default = new(() => CreateDefault());

        private bool _useWebDefaults;
        private float _pointScaleFactor = 1.0f;
        private Errata _errata = Errata.None;
        private YGLogger? _logger;
        private object? _context;
        private Dictionary<ExperimentalFeature, bool> _experimentalFeatures = new();
        private YGCloneNodeFunc? _cloneNodeFunc;

        private static YGLogger? _defaultLogger;

        public static Config Default => _default.Value;

        private static Config CreateDefault()
        {
            var config = new Config();
            config._logger = GetDefaultLoggerInternal();
            return config;
        }

        private static YGLogger GetDefaultLoggerInternal()
        {
            return (config, node, level, format, args) => { };
        }

        public static YGLogger GetDefaultLogger()
        {
            _defaultLogger ??= GetDefaultLoggerInternal();
            return _defaultLogger;
        }

        public bool UseWebDefaults
        {
            get => _useWebDefaults;
            set => _useWebDefaults = value;
        }

        public float PointScaleFactor
        {
            get => _pointScaleFactor;
            set
            {
                if (value < 0.0f)
                    throw new ArgumentException("Scale factor should not be less than zero");
                _pointScaleFactor = value;
            }
        }

        public Errata Errata
        {
            get => _errata;
            set => _errata = value;
        }

        public YGLogger? Logger
        {
            get => _logger;
            set => _logger = value ?? GetDefaultLogger();
        }

        public object? Context
        {
            get => _context;
            set => _context = value;
        }

        public bool IsExperimentalFeatureEnabled(ExperimentalFeature feature)
        {
            return _experimentalFeatures.TryGetValue(feature, out var enabled) && enabled;
        }

        public void SetExperimentalFeatureEnabled(ExperimentalFeature feature, bool enabled)
        {
            _experimentalFeatures[feature] = enabled;
        }

        public YGCloneNodeFunc? CloneNodeFunc
        {
            get => _cloneNodeFunc;
            set => _cloneNodeFunc = value;
        }

        public void Log(Node node, LogLevel level, string format, params object[] args)
        {
            _logger?.Invoke(this, node, level, format, args);
        }

        public void Dispose()
        {
            _experimentalFeatures.Clear();
        }
    }
}

