﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gapotchenko.FX.Data.Encoding
{
    /// <summary>
    /// Defines the interface of a numeric binary-to-text encoding.
    /// </summary>
    public interface INumericTextDataEncoding : ITextDataEncoding, INumericTextDataEncodingTrait
    {
    }
}