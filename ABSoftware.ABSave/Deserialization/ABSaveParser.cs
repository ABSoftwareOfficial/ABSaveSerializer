using ABParse;
using ABSoftware.ABSave.Exceptions;
using ABSoftware.ABSave.Exceptions.Base;
using ABSoftware.ABSave.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Deserialization
{
    /// <summary>
    /// Parses ABSave - powered by ABParser.
    /// </summary>
    public class ABSaveParser<T> : ABParser
    {
        #region Private Fields
        /// <summary>
        /// The ABSaveType for this parser.
        /// </summary>
        ABSaveType _abSaveType;
        #endregion

        #region Public Fields

        #region General
        /// <summary>
        /// The final object!
        /// </summary>
        public T Result;

        /// <summary>
        /// Used in the unnamed ABSave - keeps track of which item we're on for EACH element in the "CurrentObjects" (to keep things in order).
        /// </summary>
        public List<int> CurrentItem;

        /// <summary>
        /// All of the objects we're currently in and their individual values (with names).
        /// Data is collected and placed here for arrays, dictionaries and object. Once we have collected all the data (ended with "EXIT OBJECT"), we will turn this data into a full object.
        /// </summary>
        public List<dynamic> CurrentObjects;

        /// <summary>
        /// The current name that we're parsing the value for.
        /// </summary>
        public string CurrentName;
        #endregion

        #region Header

        /// <summary>
        /// The type for the ABSave (unnamed/named)
        /// </summary>
        public ABSaveType ABSaveType
        {
            get { return _abSaveType; }
            set
            {
                // Set the backing field.
                _abSaveType = value;

                // Now, if it got set to "WithOutNames" - make sure that "OnValue" stays true.
                if (value == ABSaveType.WithOutNames)
                    OnValue = true;

                // Otherwise, make it go back to a name.
                else if (value == ABSaveType.WithNames)
                    OnValue = false;
            }
        }

        /// <summary>
        /// Whether the item this parser is parsing has types.
        /// </summary>
        public bool HasTypes;

        /// <summary>
        /// The parsed version number.
        /// </summary>
        public int Version;

        #endregion

        #region Location Tracking

        /// <summary>
        /// Whether we've established the header.
        /// </summary>
        public bool EstablishedHeader = false;

        /// <summary>
        /// Whether we're on the value or not.
        /// </summary>
        public bool OnValue = false;

        /// <summary>
        /// Whether we're on the key of a dictionary or not.
        /// </summary>
        public bool OnDictionaryValue = false;

        /// <summary>
        /// Whether we're in an array currently.
        /// </summary>
        public bool InArray = false;

        /// <summary>
        /// Whether we're in a dictionary currently.
        /// </summary>
        public bool InDictionary = false;

        #endregion

        #region Other

        /// <summary>
        /// The main error handler for this parser.
        /// </summary>
        public ABSaveErrorHandler ErrorHandler;

        #endregion

        #endregion

        #region Initialization
        /// <summary>
        /// Creates a new ABSaveParser with all the correct configuration.
        /// </summary>
        /// <param name="objType">The type of the object to parse.</param>
        /// <param name="type">The way to handle the ABSave string.</param>
        /// <param name="errorHandler">The error handler for it.</param>
        public ABSaveParser(ABSaveType type, ABSaveErrorHandler errorHandler = null)
        {
            // Set all the tokens for the ABSave.
            Tokens = new System.Collections.ObjectModel.ObservableCollection<ABParserToken>()
            {
                new ABParserToken(nameof(ABSaveTokens.NextItem), '\u0001'),
                new ABParserToken(nameof(ABSaveTokens.Null), '\u0002'),
                new ABParserToken(nameof(ABSaveTokens.StartObject), '\u0003'),
                new ABParserToken(nameof(ABSaveTokens.StartArray), '\u0004'),
                new ABParserToken(nameof(ABSaveTokens.ExitObject), '\u0005'),
                new ABParserToken(nameof(ABSaveTokens.StartDictionary), '\u0006'),
            };

            // Set the correct error handler.
            ErrorHandler = ABSaveErrorHandler.EnsureNotNull(errorHandler, (e) => IsProcessing = false);

            // Set the type for now.
            ABSaveType = type;
        }

        protected override void OnStart()
        {
            // Run the default ABParser code to initialize everything.
            base.OnStart();

            // Reset the CurrentObjects.
            CurrentObjects = new List<object>();

            // Reset the CurrentItems.
            CurrentItem = new List<int>();

            // Now, configure the parent object.
            AddItem(typeof(T), ref CurrentObjects);
        }
        #endregion

        #region Main Functionality

        /// <summary>
        /// Runs when a token is processed.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTokenProcessed(TokenProcessedEventArgs e)
        {
            // Run all the default code to allow ABParser to run.
            base.OnTokenProcessed(e);

            // If we haven't established the header, this is probably the "Next Item" token which follows it, so, figure out the header based on the leading (the rest of this method uses the trailing, so we can use this token twice).
            if (!EstablishedHeader)
                EstablishHeader(e);

            // Run the correct code based on what the token is.
            switch (e.Token.Name)
            {
                // NULL ITEM
                case nameof(ABSaveTokens.Null):

                    //// If we're on the name and there's a NULL character, this isn't valid.
                    //if (!OnValue)
                    //    ErrorHandler.UnexpectedTokenWhenParsing(CurrentLocation, "A NULL character was found on a name.");

                    // Now, we're probably on the name, since if you think about it, the name would be followed by \u0002 if the value is null (e.g. Hello\u0002 would set "Hello" to null).
                    // So, since the value is already null in our outline (CurrentObjects) - we don't need to do ANYTHING except move forward one!
                    CurrentItem[CurrentItem.Count - 1]++;
                    break;

                // NEXT ITEM
                case nameof(ABSaveTokens.NextItem):

                    // Go onto the next item.
                    NextItem(e);
                    break;

                // START OBJECT
                case nameof(ABSaveTokens.StartObject):

                    // If we were on a name, and we came across this, there's a problem, because, it's technically AFTER the name that this token is found.
                    if (!OnValue && ABSaveType == ABSaveType.WithNames)
                        ErrorHandler.UnexpectedTokenWhenParsing(CurrentLocation, "The character for starting an object was found where a name should have been placed.");

                    // Also, if we haven't finished on deciding 

                    break;

                // EXIT OBJECT
                case nameof(ABSaveTokens.ExitObject):

                    // This will finish up with the current object and actually turn it into an instance!
                    FinishObject();

                    // Also, go onto the next item.

                    break;
            }
        }

        private void NextItem(TokenProcessedEventArgs e)
        {
            // If we're on a name, and we're in a named ABSave, get the name and place it into "CurrentName".
            if (!OnValue && ABSaveType == ABSaveType.WithNames)
                SetCurrentName(e.Trailing);

            // Otherwise, if we were on a name, we're now on a value.
            else
            {
                // We want to remember what index the item is at (particually when named) to save performance.
                var index = (ABSaveType == ABSaveType.WithNames) ? CurrentObjects.Last().ObjectItems.FindIndex(CurrentName) : CurrentItem.Last();

                // Parse the value, and get the result - we'll only "manuallyParse" for errors because if it was a different type, it would have an "EXIT OBJECT" at the end - not a "NEXT ITEM".
                var result = ABSaveDeserializer.Deserialize(e.Trailing, CurrentObjects.Last().ObjectItems.Items[index].Info.FieldType, out ABSavePrimitiveType determinedType, out bool manuallyParse, ErrorHandler, CurrentLocation);

                //// If it determined it as an object, we'll take that and add it as a new object.
                //if (determinedType == ABSavePrimitiveType.Object)
                //{
                //    // If the result is null - that means it couldn't get a valid type, so there's a problem.
                //    if (result == null)
                        
                //}

                // So, if needs to be manually parsed - and it isn't an object, there's a problem.
                if (manuallyParse && determinedType != ABSavePrimitiveType.Object)
                    ErrorHandler.InvalidValueInABSaveWhenParsing(CurrentLocation, "The ABSave string '" + e.Leading + "' is not valid for the type '" + CurrentObjects.Last().Type.ToString() + "'.");

                // Now, set the value, at the correct location.
                SetCurrentValue(index, result);
            }
        }

        protected override void OnEnd()
        {
            // If any objects haven't been closed yet, do that now - because, ABSave doesn't leave trailing "EXIT OBJECT"s.
            while (CurrentObjects.Count != 0)
                FinishObject();
        }

        /// <summary>
        /// Runs when we come across a name, this will set the "CurrentName" variable correctly.
        /// </summary>
        /// <param name="name">The name</param>
        public void SetCurrentName(string name)
        {
            CurrentName = name;

            // If we've gone forward too far in a object, there's a problem with the ABSave.
            if (CurrentItem.Last() > CurrentObjects.Last().ObjectItems.Count)
                ErrorHandler.MoreNextItemTokensThanItems(CurrentLocation);

            // Mark us as now being on a value.
            OnValue = true;
        }

        /// <summary>
        /// Runs when we come across a value, this will set an object to the CurrentObject.
        /// </summary>
        /// <param name="obj">The object to set.</param>
        public void SetCurrentValue(int location, object obj)
        {
            // If it's named and no name was provided, there's a problem.
            if (ABSaveType == ABSaveType.WithNames && string.IsNullOrEmpty(CurrentName))
                ErrorHandler.MissingNameToValueWhenParsing(CurrentLocation);

            // If we got here, go ahead and add the value, the location is already provided to save performance.
            CurrentObjects.Last().ObjectItems.Items[location].Value = obj;

            // Move forward one item.
            CurrentItem[CurrentItem.Count - 1]++;

            // Mark us as now being on a name again for the item.
            OnValue = false;
        }
        #endregion

        #region Components

        #region Header
        /// <summary>
        /// Figures out the header.
        /// </summary>
        /// <param name="e"></param>
        public void EstablishHeader(TokenProcessedEventArgs e)
        {
            // If the token ISN'T a "Next Item" token, the header is obviously incorrect.
            if (e.Token.Name != nameof(ABSaveTokens.NextItem))
            {
                ErrorHandler.InvalidHeaderWhenParsing("The header does not appear to follow the basic layout of 'U' or 'N' followed by a number.");
                return;
            }

            // First of all, if there isn't a leading, there is no header!
            if (string.IsNullOrEmpty(e.Leading))
            {
                // Because there's no header, the ABSaveType won't be found out... So, if it's set to "infer" then obviously the type given is invalid and can't be worked out.
                if (ABSaveType == ABSaveType.Infer)
                    ErrorHandler.InvalidHeaderWhenParsing("This parser was given the ABSaveType of 'infer'... However, there is no header so it can't infer anything.");

                // Now, there's nothing more to do here.
                return;
            }

            // If we're here, there is header, however, it must be 2 or more characters.
            if (e.Leading.Length < 2)
            {
                ErrorHandler.InvalidHeaderWhenParsing("The header was too short to be valid - and it wasn't empty either");
                return;
            }

            // Lowercase the leading to make comparing easier.
            var leading = e.Leading.ToLower();

            // If we need to get the type from the header - do it.
            if (ABSaveType == ABSaveType.Infer)
                GetTypeFromHeader(leading);

            // Next, we need to get the "Write Type" bool.
            GetShowTypesFromHeader(leading);

            // Now, if there's more to the header - the rest should all be the version number.
            if (leading.Length > 2)
            {
                // Attempt to parse the version number.
                bool passed = int.TryParse(leading.Substring(2, leading.Length - 2), out int version);

                // If it failed, throw an error.
                if (!passed)
                {
                    ErrorHandler.InvalidHeaderWhenParsing("The version number following the type and show types identifiers is not valid - remember it should only be an integer.");
                    return;
                }

                // Otherwise, set the final version number.
                Version = version;
            }

            // If we made it here, we've established the header.
            EstablishedHeader = true;
        }

        /// <summary>
        /// Gets a type from the header - called from <see cref="EstablishedHeader"/>.
        /// </summary>
        /// <param name="leading">The lowercase leading.</param>
        private void GetTypeFromHeader(string leading)
        {
            // Get the first character - now that we've confirmed there's at least ONE character in the leading.
            var firstChar = leading[0];

            // If the character is a 'U' - that's unnamed.
            if (firstChar == 'u')
                ABSaveType = ABSaveType.WithOutNames;

            // If it's an 'N' - that's named.
            else if (firstChar == 'n')
                ABSaveType = ABSaveType.WithNames;

            // However, if the first character isn't any of them, it's invalid.
            else
            {
                ErrorHandler.InvalidHeaderWhenParsing("The first character of the header provided is not valid ('U' or 'N').");
                return;
            }

            // We're all done, so go back to main "EstablishHeader" method!
            return;
        }

        /// <summary>
        /// Gets whether this string should show types from the header - called from <see cref="EstablishedHeader"/>.
        /// </summary>
        /// <param name="leading">The lowercase leading for the header.</param>
        private void GetShowTypesFromHeader(string leading)
        {
            HasTypes = ABSaveDeserializer.DeserializeBool(leading[1].ToString(), ErrorHandler, 1);
        }

        #endregion

        /// <summary>
        /// Adds a dictionary item to a certain dictionary with a type and all the keys/values along with it.
        /// </summary>
        public void AddItem(Type type, ref List<object> addTo)
        {
            // Get all the fields/properties in the type.
            var variables = ABSaveUtils.GetFieldsAndPropertiesWithTypes(type);

            // Now, add them to the main dictionary.
            addTo.Add(new ABSaveParserItem<T, object, object, object>(variables));

            // Also, add a new item to "CurrentItem" for this object.
            CurrentItem.Add(0);
        }

        /// <summary>
        /// Finishes an item by actually creating a final instance out of it.
        /// </summary>
        public void FinishObject()
        {
            // Create the object based on the data we've collected in the CurrentObjects.
            var obj = ABSaveUtils.CreateInstance(CurrentObjects.Last().GetObjectType(), ErrorHandler, CurrentObjects.Last().ObjectItems);

            // Remove the last object from the CurrentObjects - since we're done with it now.
            CurrentObjects.RemoveAt(CurrentObjects.Count - 1);

            // Remove the last item index from the CurrentItem - since we're done with that now as well.
            CurrentItem.RemoveAt(CurrentItem.Count - 1);

            // Now, if we've gotten rid of ALL the CurrentObjects, place the object we just made in the final "Result".
            if (CurrentObjects.Count == 0)
            {
                Result = obj;
                return;
            }

            // If it isn't the last one, we need to put the final object where it's meant to go, which is just where the "CurrentItem" is NOW, because we've removed the last one from it.
            CurrentObjects.Last().ObjectItems.Items[CurrentItem.Last()] = obj;
        }

        #endregion
    }
}
