//------------------------------------------------------------------------------
// <copyright file="StateCapture.cs" company="Florian Wolters">
//     Copyright (c) Florian Wolters. All rights reserved.
// </copyright>
// <author>Florian Wolters &lt;wolters.fl@gmail.com&gt;</author>
// <source>https://github.com/FlorianWolters/component-based-authoring-add-in-for-microsoft-word/blob/master/FlorianWolters.Office.Word/StateCapture.cs</source>
//------------------------------------------------------------------------------

namespace AnalysisManager.Models
{
    using System;
    using Word = Microsoft.Office.Interop.Word;

    /// <summary>
    /// The class <see cref="StateCapture"/> captures the state of the <see cref="Word.Application"/> for a <see
    /// cref="Word.Document"/>.
    /// </summary>
    /// <remarks>
    /// The idea for this class has been taken from
    /// <a href="http://clear-lines.com/blog/post/Wrapping-long-running-Excel-operations-with-IDisposable.aspx>">this</a>
    /// article.
    /// </remarks>
    public class StateCapture : IDisposable
    {
        /// <summary>
        /// Determines whether screen updating is initially turned on in the <see cref="Word.Application"/>.
        /// </summary>
        private readonly bool initialScreenUpdating;

        /// <summary>
        /// The <see cref="Word.Application"/> whose state to capture.
        /// </summary>
        private Word.Application application;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateCapture"/> class.
        /// </summary>
        /// <param name="document">
        /// The <see cref="Word.Document"/> which holds a reference to the <see cref="Word.Application"/> whose
        /// state to capture.
        /// </param>
        public StateCapture(Word.Document document)
        {
            this.application = document.Application;
            this.initialScreenUpdating = this.application.ScreenUpdating;
            this.application.ScreenUpdating = false;
        }

        /// <summary>
        /// Resets the state of the <see cref="Word.Application"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this <see cref="StateCapture"/> and optionally releases the managed
        /// resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (null != this.application)
                {
                    this.application.ScreenUpdating = this.initialScreenUpdating;
                }

                this.application = null;
            }
        }
    }
}
