namespace Sisusa.DateBasedIdGenerator;

public class DateBasedSerialGenerator
{
    public static long Generate(DateTime currentDateTime, int minSerial=1, int maxSerial=999)
    {
        return new DateBasedSerial(currentDateTime, Random.Shared.Next(minSerial, maxSerial))
            .UseFiller(1100)
            .Generate();
    }

    public static DateBasedSerial GetGenerator(DateTime currentDateTime, int minSerial = 1, int maxSerial = 999)
    {
        return new DateBasedSerial(currentDateTime, Random.Shared.Next(minSerial, maxSerial));
    }
}

/// <summary>
/// Simple class to generate serials of the form yyyymmddxxxxsss (202505112200102)
/// or yymmddxxxxsss (2505112200102)[default] can even include the time for longer randoms.
/// </summary>
/// <param name="currentDateTime">Date on which to base the serial.</param>
/// <param name="serial">Serial number to append to the date.</param>


public class DateBasedSerial(DateTime currentDateTime, int serial)
{
    /// <summary>
    /// Should the serial include the time (HH:mm)?
    /// </summary>
    private bool _includeTime = false;
    
    /// <summary>
    /// Should the serial use two digits for the year?
    /// </summary>
    private bool _twoDigitYear = true;
    
    private int _filler = 0;
    
    /// <summary>
    /// The attached Date
    /// </summary>
    private DateTime Date { get; init; } = currentDateTime;
    
    /// <summary>
    /// The serial to append to the date.
    /// </summary>
    private int Serial => serial;

    /// <summary>
    /// Sets to include the time "hh:mm" in the generated serial.
    /// </summary>
    /// <returns>Current instance for further operations.</returns>
    public DateBasedSerial IncludeTime()
    {
        if (_includeTime)
            return this;
        _includeTime = true;
        return this;
    }

    /// <summary>
    /// Sets to use two digits for the year.
    /// </summary>
    /// <returns>Current instance.</returns>
    // public DateBasedSerial UseShortYear()
    // {
    //     if (!_twoDigitYear)
    //         _twoDigitYear = true;
    //     return this;
    // }

    /// <summary>
    /// Sets a filler or identifier to use to separate the generated serial from others.
    /// </summary>
    /// <param name="filler">The filter or identifier</param>
    public DateBasedSerial UseFiller(int filler)
    {
        _filler = filler;
        return this;
    }

    /// <summary>
    /// Sets to use the full four-digit year in the serial.
    /// </summary>
    public DateBasedSerial UseFullYear()
    {
        if (_twoDigitYear)
        {
            _twoDigitYear = false;
        }
        return this;
    }

    /// <summary>
    /// Generates a pseudo random date-based number from the given parameters/
    /// </summary>
    /// <returns>The generated number</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the generated number exceeds the <see cref="long"/> maxValue.</exception>
    public long Generate()
    {
        var yrStr = _twoDigitYear ? "yy" : "yyyy";
        var dateStr = Date.ToString(_includeTime ? $"{yrStr}MMddHHmm" : $"{yrStr}MMdd");
        const int maxLongLen = 19;
        var  dateStrLen = dateStr.Length;
        var fillerLen = _filler > 0 ? _filler.ToString().Length : 0;
        var maxSerialLen = maxLongLen - dateStrLen - fillerLen;
        Console.WriteLine($"Allowed serial length: {maxSerialLen}");
        if (Serial >= Math.Pow(10, maxSerialLen))
        {
            throw new ArgumentOutOfRangeException($"Serial {Serial} exceeds the maximum length of `int64` => {maxLongLen}");
        }
        var serialStr = _filler > 0 ?  $"{dateStr}{_filler:d4}{Serial:d4}": $"{dateStr}{Serial:d4}";

        if (long.TryParse(serialStr, out var parsedSerial))
            return parsedSerial;
        throw new ArgumentOutOfRangeException(
            $"Generated serial {serialStr} exceeds the maximum value of `int64` {long.MaxValue}");
    }
}