using System.Globalization;
using LambdaKit.Logging;
using LambdaKit.Logging.Targets;
using LambdaKit.Terminal;

namespace GAIL.Core;

public static class LoggerFactory {
    public static void ApplySettings(Logger logger) {
        int index = logger.GetTargetIndex<TerminalTarget>();
        if (index > -1) {
            logger.GetTarget<TerminalTarget>(index)!.Format =
                new StyleBuilder().Text("<{0}>: (")
                .Foreground((StandardColor)StandardColor.Colors.Blue).Text("{2}")
                .Reset().Text(")[{5}").Bold().Text("{3}").Bold(false).ResetForeground()
                .Text("] : {5}{4}").Reset().ToString();
            logger.GetTarget<TerminalTarget>(index)!.NameFormat =  "{0} - {1}";
        }
        index = logger.GetTargetIndex<FileTarget>();
        if (index > -1) {
            logger.GetTarget<FileTarget>(index)!.Format = "<{0}>: ({2})[{3}] : {4}";
            logger.GetTarget<FileTarget>(index)!.NameFormat =  "{0} - {1}";
        }
    }
    public static Logger Create(string name, Logger? inputtedLogger = null, Severity? severity = null) {
        if (severity == null) {
            #if DEBUG
            severity = Severity.Trace;
            #else
            severity = Severity.Info;
            #endif
        }
        inputtedLogger ??= new Logger(name, severity:severity.Value, targets:[new TerminalTarget()]);
        ApplySettings(inputtedLogger);
        return inputtedLogger;
    }
    public static SubLogger CreateSublogger(Logger parentLogger, string name, string id) {
        SubLogger subLogger = parentLogger.CreateSubLogger(id, name, false, severity:parentLogger.logLevel, targets: [.. parentLogger.Targets.Select(x => x.target)]);
        ApplySettings(subLogger);
        return subLogger;
    }
}