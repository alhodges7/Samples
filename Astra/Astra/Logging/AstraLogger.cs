using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Astra.Logging
{
  [Serializable]
  public enum LogLevel
  {
    Debug = 0,
    Info = 1,
    Error = 2,
    Fatal = 3
  }

  /// <summary>
  /// Class that safely wraps logging methods for the Log4Net library, as implemented
  /// in the Astra system.
  /// </summary>
  public static class AstraLogger
  {
    /// <summary>
    /// Logs an information message to the EventLog table.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    public static void LogInfo(string message)
    {
      AstraLogger.LogInfo(AstraLogger.CallingMethodType(), message);
    }

    /// <summary>
    /// Logs an information message to the EventLog table.
    /// </summary>
    /// <param name="sender">The sender of the request to log information.</param>
    /// <param name="message">The message to be logged.</param>
    public static void LogInfo(object sender, string message)
    {
      AstraLogger.LogInfo(sender.GetType(), message);
    }

    /// <summary>
    /// Logs an information message to the EventLog table.
    /// </summary>
    /// <param name="senderType">The type of object that is sending the request for logging.</param>
    /// <param name="message">The message to be logged.</param>
    public static void LogInfo(Type senderType, string message)
    {
      log4net.ILog log = log4net.LogManager.GetLogger(senderType);
      log.Info(message);
    }

    /// <summary>
    /// Logs an debug (Trace) message to the EventLog table.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    public static void LogDebug(string message)
    {      
      AstraLogger.LogDebug(AstraLogger.CallingMethodType(), message);
    }

    /// <summary>
    /// Logs an debug (Trace) message to the EventLog table.
    /// </summary>
    /// <param name="sender">The sender of the request to log information.</param>
    /// <param name="message">The message to be logged.</param>
    public static void LogDebug(object sender, string message)
    {
      AstraLogger.LogDebug(sender.GetType(), message);
    }

    /// <summary>
    /// Logs an debug (Trace) message to the EventLog table.
    /// </summary>
    /// <param name="senderType">The type of object that is sending the request for logging.</param>
    /// <param name="message">The message to be logged.</param>
    public static void LogDebug(Type senderType, string message)
    {
      log4net.ILog log = log4net.LogManager.GetLogger(senderType);
      log.Debug(message);
    }

    /// <summary>
    /// Logs an Error message to the EventLog table.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    public static void LogError(string message)
    {
      AstraLogger.LogError(AstraLogger.CallingMethodType(), message);
    }

    /// <summary>
    /// Logs an Error message to the EventLog table.
    /// </summary>
    /// <param name="sender">The sender of the request to log information.</param>
    /// <param name="message">The message to be logged.</param>
    public static void LogError(object sender, string message)
    {
      AstraLogger.LogError(sender.GetType(), message);
    }
   

    /// <summary>
    /// Logs an Error message to the EventLog table.
    /// </summary>
    /// <param name="e">The exception that will be logged.</param>
    public static void LogError(Exception e)
    {
      log4net.ILog log = log4net.LogManager.GetLogger(AstraLogger.CallingMethodType());
      log.Error(e.Message, e);
    }

    /// <summary>
    /// Logs an Error message to the EventLog table.
    /// </summary>
    /// <param name="senderType">The type of object that is sending the request for logging.</param>
    /// <param name="message">The message to be logged.</param>
    public static void LogError(Type senderType, string message)
    {
      log4net.ILog log = log4net.LogManager.GetLogger(senderType);
      log.Error(message);
    }

    /// <summary>
    /// Logs a Fatal-level error message to the EventLog table.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    public static void LogFatal(string message)
    {
      log4net.ILog log = log4net.LogManager.GetLogger(AstraLogger.CallingMethodType());
      log.Fatal(message);
    }

    /// <summary>
    /// Logs a Fatal-level error message to the EventLog table.
    /// </summary>
    /// <param name="sender">The sender of the request to log information.</param>
    /// <param name="message">The message to be logged.</param>
    public static void LogFatal(object sender, string message)
    {
      AstraLogger.LogFatal(sender.GetType(), message);
    }

    /// <summary>
    /// Logs a Fatal-level error message to the EventLog table.
    /// </summary>
    /// <param name="senderType">The type of object that is sending the request for logging.</param>
    /// <param name="message">The message to be logged.</param>
    public static void LogFatal(Type senderType, string message)
    {
      log4net.ILog log = log4net.LogManager.GetLogger(senderType);
      log.Fatal(message);
    }

    /// <summary>
    /// Logs a Fatal-level error message to the EventLog table.
    /// </summary>
    /// <param name="e">The exception that will be logged.</param>
    public static void LogFatal(Exception e)
    {
      log4net.ILog log = log4net.LogManager.GetLogger(AstraLogger.CallingMethodType());
      log.Fatal(e.Message, e);
    }

    /// <summary>
    /// Logs a Fatal-level error message to the EventLog table.
    /// </summary>
    /// <param name="senderType">The type of object that is sending the request for logging.</param>
    /// <param name="e">The exception that will be logged.</param>
    public static void LogFatal(Type senderType, Exception e)
    {
      log4net.ILog log = log4net.LogManager.GetLogger(senderType);
      log.Fatal(e.Message, e);
    }

    /// <summary>
    /// Tests a given condition and, if the condition is FALSE, logs a
    /// message to the EventLog table.
    /// </summary>
    /// <param name="testCondition">The condition to test.</param>
    /// <param name="message">The message to write to the log.</param>
    public static void Assert(bool testCondition, string message)
    {
      AstraLogger.Assert(testCondition, message, LogLevel.Debug);
    }

    /// <summary>
    /// Tests a given condition and, if the condition is FALSE, logs a
    /// message to the EventLog table.
    /// </summary>
    /// <param name="testCondition">The condition to test.</param>
    /// <param name="message">The message to write to the log.</param>
    /// <param name="logLevel">The log level at which to write the log.</param>
    public static void Assert(bool testCondition, string message, LogLevel logLevel)
    {
      if (testCondition)
        return;

      Type callingMethodType = CallingMethodType();

      switch (logLevel)
      {
        case LogLevel.Debug:
          AstraLogger.LogDebug(callingMethodType, message);
          break;

        case LogLevel.Info:
          AstraLogger.LogInfo(callingMethodType, message);
          break;

        case LogLevel.Error:
          AstraLogger.LogError(callingMethodType, message);
          break;

        case LogLevel.Fatal:
          AstraLogger.LogFatal(callingMethodType, message);
          break;
      }

      return;

    }

    private static Type CallingMethodType()
    {
      Type decType = null;

      try
      {
        StackFrame frame = new StackFrame(2);
        StackTrace trace = new StackTrace(frame);
        var method = frame.GetMethod();
        decType = method.DeclaringType;
      }
      catch
      {
        return typeof(AstraLogger);
      }

      if (decType == null)
        return typeof(AstraLogger);
      else
        return decType;      
      
    }
  }
}