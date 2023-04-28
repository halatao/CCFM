using CiscoConfigurationFileManager.Services.Configurations;
using Reactive.Bindings;
using System;
using System.Reactive.Linq;

namespace CiscoConfigurationFileManager.Models;

/// <summary>
/// A wrapper class for a single line of configuration.
/// </summary>
public class LineWrapper
{
    /// <summary>
    /// The value of the configuration line.
    /// </summary>
    public ReactiveProperty<string> Line { get; set; }

    /// <summary>
    /// The operation to perform on the configuration line.
    /// </summary>
    public OperationEnum OperationEnum { get; set; } = OperationEnum.None;

    /// <summary>
    /// Indicates whether the line is an original line or not.
    /// </summary>
    public bool Original { get; set; } = true;

    /// <summary>
    /// Indicates whether the line is valid or not.
    /// </summary>
    public ReactiveProperty<bool> Valid { get; set; }

    /// <summary>
    /// Creates a new instance of the <see cref="LineWrapper"/> class.
    /// </summary>
    public LineWrapper()
    {
        Valid = new ReactiveProperty<bool>(true);
        Line = new ReactiveProperty<string>();
        Line.Subscribe(s =>
        {
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => { ValidateLine(); });
        });
    }

    /// <summary>
    /// Performs an operation on the configuration line.
    /// </summary>
    /// <param name="operationEnum">The operation to perform.</param>
    /// <param name="optional">An optional value for the operation.</param>
    public void Operate(OperationEnum operationEnum, string optional = "empty")
    {
        const string empty = "empty";
        switch (operationEnum)
        {
            case OperationEnum.Override when optional != empty:
                Line.Value = optional;
                OperationEnum = OperationEnum.Override;
                break;
            case OperationEnum.Delete when optional == empty:
                OperationEnum = OperationEnum.Delete;

                if (Line.Value.StartsWith("no"))
                {
                    break;
                }

                Line.Value = "no " + Line.Value;
                break;
            case OperationEnum.None:
                OperationEnum = OperationEnum.None;
                break;
            default:
                OperationEnum = OperationEnum.None;
                break;
        }
    }

    /// <summary>
    /// Validates the configuration line.
    /// </summary>
    public void ValidateLine()
    {
        Valid.Value = CommandValidator.ValidateLine(Line.Value);
    }
}