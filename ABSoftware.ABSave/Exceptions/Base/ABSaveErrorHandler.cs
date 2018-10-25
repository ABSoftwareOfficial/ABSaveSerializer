using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Exceptions.Base
{
    /// <summary>
    /// A way of controlling how ABSave handles errors.
    /// </summary>
    public class ABSaveErrorHandler
    {
        /// <summary>
        /// A default ABSaveErrorHandler with no special configuration.
        /// </summary>
        public static ABSaveErrorHandler Default;

        /// <summary>
        /// Makes sure that a error handler isn't null, if it is create a new basic one with no suppression.
        /// </summary>
        /// <param name="errorHandler">The errorHandler that's been passed in - this is what gets checked.</param>
        /// <param name="whenErrorEncountered">Adds code to run when an error is thrown.</param>
        public static ABSaveErrorHandler EnsureNotNull(ABSaveErrorHandler errorHandler, ErrorEncounteredEventHandler whenErrorEncountered = null)
        {
            // If it's null, use a default one.
            if (errorHandler == null)
                if (whenErrorEncountered == null)
                    return Default;
                else
                    return new ABSaveErrorHandler(0, whenErrorEncountered);
            else
            {
                // Otherwise, add the event to it and then return it.
                if (whenErrorEncountered != null)
                    errorHandler.ErrorEncountered += whenErrorEncountered;

                return errorHandler;
            }
        }

        /// <summary>
        /// Which errors to suppress down to the event (and stopping the process).
        /// </summary>
        public ABSaveError SuppressedErrors;

        /// <summary>
        /// Whether we should just completely ignore all errors. NOT RECOMMENDED - CAN CAUSE WEIRD EFFECTS.
        /// </summary>
        public bool IgnoreAllErrors = false;

        #region Event
        
        public delegate void ErrorEncounteredEventHandler(ErrorEncounteredEventArgs e);

        /// <summary>
        /// When an error has been encountered.
        /// </summary>
        public event ErrorEncounteredEventHandler ErrorEncountered = (e) => { };
        #endregion

        #region Errors
        /// <summary>
        /// This is ran when an "Infer Type" is passed when serializing.
        /// </summary>
        public void InferTypeWhenSerializing()
        {
            // Create EventArgs to pass around information about what happened.
            var args = new ErrorEncounteredEventArgs(ABSaveError.InferTypeWhenSerializing, 0);

            // Now, handle the error.
            HandleError(args, ABSaveError.InferTypeWhenSerializing, new InferABSaveTypeWhenSerializing(args.ToString()));
        }

        /// <summary>
        /// This is ran when an invalid header is found when parsing.
        /// </summary>
        public void InvalidHeaderWhenParsing(string message)
        {
            // Create EventArgs to pass around information about what happened.
            var args = new ErrorEncounteredEventArgs(ABSaveError.InvalidHeaderWhenParsing, 0, message);

            // Now, handle the error.
            HandleError(args, ABSaveError.InvalidHeaderWhenParsing, new InvalidHeaderWhenParsing(args.ToString()));
        }

        /// <summary>
        /// This is ran when too many constructors were found when attempting to create an object.
        /// </summary>
        public void TooManyConstructorsWithDifferentCase(Type type, int location)
        {
            // Create EventArgs to pass around information about what happened.
            var args = new ErrorEncounteredEventArgs(ABSaveError.TooManyConstructorsWithDifferentCase, location, "When creating an instance of an object, there were too many constructors with matching names to the fields/properties (when ignoring case)... As a result, ABSave cannot determine which constructor to use. This is with the type '" + type.ToString() + "'.");

            // Now, handle the error.
            HandleError(args, ABSaveError.TooManyConstructorsWithDifferentCase, new TooManyConstructorsWithDifferentCase(args.ToString()));
        }

        /// <summary>
        /// This is ran when a valid constructor can not be found when creating an instance of an object.
        /// </summary>
        public void InvalidConstructorsForCreatingObject(Type type, int location)
        {
            // Create EventArgs to pass around information about what happened.
            var args = new ErrorEncounteredEventArgs(ABSaveError.InvalidConstructorsForCreatingObject, location, "A valid constructor cannot be found for the type: " + type.ToString());

            // Now, handle the error.
            HandleError(args, ABSaveError.InvalidConstructorsForCreatingObject, new InvalidConstructorsWhenCreatingObject(args.ToString()));
        }

        /// <summary>
        /// This is ran when there is an unexpected token when parsing.
        /// </summary>
        public void UnexpectedTokenWhenParsing(int location, string message)
        {
            // Create EventArgs to pass around information about what happened.
            var args = new ErrorEncounteredEventArgs(ABSaveError.UnexpectedTokenWhenParsing, location, message);

            // Now, handle the error.
            HandleError(args, ABSaveError.UnexpectedTokenWhenParsing, new UnexpectedTokenWhenParsing(args.ToString()));
        }

        /// <summary>
        /// This is ran when there are more "Next Item" tokens than actual items.
        /// </summary>
        public void MoreNextItemTokensThanItems(int location)
        {
            // Create EventArgs to pass around information about what happened.
            var args = new ErrorEncounteredEventArgs(ABSaveError.MoreNextItemTokensThanItems, location, "There were more 'next item' tokens than there were fields/properties on an object.");

            // Now, handle the error.
            HandleError(args, ABSaveError.MoreNextItemTokensThanItems, new MoreNextItemTokensThanActualItems(args.ToString()));
        }

        /// <summary>
        /// This is ran when there is an invalid string representing data.
        /// </summary>
        /// <param name="location"></param>
        public void InvalidValueInABSaveWhenParsing(int location, string message)
        {
            // Create EventArgs to pass around information about what happened.
            var args = new ErrorEncounteredEventArgs(ABSaveError.InvalidValueInABSaveWhenParsing, location, message);

            // Now, handle the error.
            HandleError(args, ABSaveError.InvalidValueInABSaveWhenParsing, new InvalidValueInABSaveWhenParsing(args.ToString()));
        }

        /// <summary>
        /// This is ran when there is a missing name for a value.
        /// </summary>
        /// <param name="location"></param>
        public void MissingNameToValueWhenParsing(int location)
        {
            // Create EventArgs to pass around information about what happened.
            var args = new ErrorEncounteredEventArgs(ABSaveError.MissingNameToValueWhenParsing, location, "A value has been found without a matching name.");

            // Now, handle the error.
            HandleError(args, ABSaveError.MissingNameToValueWhenParsing, new MissingNameToValueWhenParsing(args.ToString()));
        }

        /// <summary>
        /// Handles all the basic functionality for an error (throw exception if needed and run "ErrorEncountered").
        /// </summary>
        /// <param name="args">The information about the error.</param>
        /// <param name="err">The type the error was.</param>
        /// <param name="exception">The actual exception this error is contained in.</param>
        internal void HandleError(ErrorEncounteredEventArgs args, ABSaveError err, Exception exception)
        {
            // Call the event "ErrorEncountered" - if we aren't meant to IgnoreAllErrors.
            if (!IgnoreAllErrors)
                ErrorEncountered?.Invoke(args);

            // If it isn't suppressed, throw an exception.
            if (!SuppressedErrors.HasFlag(err))
                throw exception;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ABSaveErrorHandler"/> which handles errors when serializing/parsing.
        /// </summary>
        public ABSaveErrorHandler() { }

        /// <summary>
        /// Creates a new <see cref="ABSaveErrorHandler"/> which handles errors when serializing/parsing.
        /// </summary>
        /// <param name="suppressed">The errors to not cause an exception.</param>
        /// <param name="whenErrorEncountered">Code to run when an error is encountered.</param>
        /// <param name="ignoreAll">NOT RECOMMENDED: Ignore ALL errors and continue on as if nothing happened. (CAN CAUSE WEIRD RESULTS)</param>
        public ABSaveErrorHandler(ABSaveError suppressed, ErrorEncounteredEventHandler whenErrorEncountered = null, bool ignoreAll = false)
        {
            // Set the suppressed errors.
            SuppressedErrors = suppressed;

            // If the action isn't null, add it to the "ErrorEncountered" event.
            if (whenErrorEncountered != null)
                ErrorEncountered += whenErrorEncountered;

            // Set the "IgnoreAllErrors".
            IgnoreAllErrors = ignoreAll;
        }
        #endregion
    }

    /// <summary>
    /// Represents info about when an error is ecountered with a certain error handler.
    /// </summary>
    public class ErrorEncounteredEventArgs : EventArgs
    {
        /// <summary>
        /// The error that was encountered.
        /// </summary>
        public ABSaveError Error;

        /// <summary>
        /// The location that this error was encountered.
        /// </summary>
        public int Location;

        /// <summary>
        /// The additional message which SOME errors may follow with.
        /// </summary>
        public string Message;

        /// <summary>
        /// Creates a new <see cref="ErrorEncounteredEventArgs"/> to pass information around about when an error was encounted via a <see cref="ABSaveErrorHandler"/>
        /// </summary>
        public ErrorEncounteredEventArgs(ABSaveError error, int charLocation, string message = "")
        {
            // Set the error/location.
            Error = error;
            charLocation = Location;

            // Set the message.
            Message = message;
        }

        /// <summary>
        /// Writes a message explaining what happened.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Error} exception was encountered at character position: {Location}" +
                (string.IsNullOrEmpty(Message) ? "" : Environment.NewLine + Environment.NewLine + "ADDITIONAL INFO: " + Message);
        }
    }
}
