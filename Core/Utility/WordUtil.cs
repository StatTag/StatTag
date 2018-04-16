using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;

namespace StatTag.Core.Utility
{
    // Our SafeGet* util methods are a mechanism to catch COM exceptions that are otherwise impossible
    // (or just really, really difficult) to detect before they are thrown.  Because of this, we are
    // ignoring what the actual exception is.
    public class WordUtil
    {
        public static Fields SafeGetFieldsFromShape(Shape shape)
        {
            try
            {
                if (shape == null || shape.TextFrame == null || shape.TextFrame.TextRange == null)
                {
                    return null;
                }

                return shape.TextFrame.TextRange.Fields;
            }
            catch
            {
                // This is a safe wrapper function, so we are eating the exception
                return null;
            }
        }

        public static ShapeRange SafeGetShapeRangeFromSelection(Selection selection)
        {
            ShapeRange shape = null;
            try
            {
                shape = selection.ShapeRange;
            }
            catch
            {
                // This is a safe wrapper function, so we are eating the exception
                shape = null;
            }

            return shape;
        }

        public static Fields SafeGetFieldsFromSelection(Selection selection)
        {
            Fields fields = null;
            try
            {
                fields = selection.Fields;
            }
            catch
            {
                // This is a safe wrapper function, so we are eating the exception
                fields = null;
            }

            return fields;
        }
    }
}
