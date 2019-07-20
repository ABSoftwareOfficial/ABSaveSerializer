using System;
using System.Collections.Generic;
using System.Text;
using ABSoftware.ABSave.Exceptions.Base;
using ABSoftware.ABSave.Helpers;

namespace ABSoftware.ABSave
{
    /// <summary>
    /// How ABSave handles things in an ABSave Document - includes all the settings EXCLUDING the ABSaveType.
    /// </summary>
    public class ABSaveSettings
    {
        #region Internal Info

        /// <summary>
        /// The publicly viewable "Indentify Types" - only the core of ABSave actually cares about <see cref="CurrentlyCanIdentifyTypes" />.
        /// </summary>
        private bool _identifyTypes = true;

        #endregion

        #region Public Settings
        /// <summary>
        /// The error handler to use.
        /// </summary>
        public ABSaveErrorHandler ErrorHandler = ABSaveErrorHandler.Default;

        /// <summary>
        /// Whether this should be done with/without types.
        /// </summary>
        public bool WithTypes { get; set; } = true;

        /// <summary>
        /// Whether the process can "remember" a types by given each unique type a key, to avoid duplication and keep the ABSave compact.
        /// </summary>
        public bool RememberTypes
        {
            get { return _identifyTypes; }
            set
            {
                _identifyTypes = value;
                CurrentlyCanIdentifyTypes = value;
            }
        }

        /// <summary>
        /// Whether this has a version for the header.
        /// </summary>
        public bool HasVersion { get; set; } = false;

        /// <summary>
        /// The version provided - if there is one.
        /// </summary>
        public int Version { get; set; } = 0;

        #endregion

        #region Identifying Types

        /// <summary>
        /// Whether we are currently in the position that we can identify types, this will not be true if we go beyond an "Int16" number of types (almost impossible).
        /// </summary>
        internal bool CurrentlyCanIdentifyTypes = true;

        /// <summary>
        /// All the types which have been given an ID.
        /// </summary>
        internal List<ABSaveIdentifiedType> RememberedTypes = new List<ABSaveIdentifiedType>();

        /// <summary>
        /// The highest number we've given an identified type.
        /// </summary>
        internal short HighestIdentifiedType = 0;

        /// <summary>
        /// Searches for an identified type.
        /// </summary>
        /// <param name="type">The type to search for.</param>
        /// <returns>The characters that make up the key for it.</returns>
        public char[] SearchForIdentifiedType(Type type)
        {
            // Go through each item in the IdentifiedTypes (starting from the end since it's more likely to be there) - and return the key if we find one.
            for (int i = HighestIdentifiedType - 1; i >= 0; i--)
                if (type == RememberedTypes[i].Type)
                    return RememberedTypes[i].WrittenKey;

            // If we didn't find anything, return null.
            return null;
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Creates ABSaveSettings with all default configuration (caching, types, no version)
        /// </summary>
        public ABSaveSettings() { }

        /// <summary>
        /// Creates ABSaveSettings with some basic configuration.
        /// </summary>
        public ABSaveSettings(bool withTypes, bool identifyTypes = true)
        {
            // Initialize all the basic parameters.
            InitializeBasicConfig(withTypes, identifyTypes);
        }

        /// <summary>
        /// Creates ABSaveSettings with some basic configuration and a version number.
        /// </summary>
        public ABSaveSettings(int version, bool withTypes, bool identifyTypes = true)
        {
            // Initialize all the basic parameters, and the version.
            InitializeBasicConfig(withTypes, identifyTypes);
            InitializeVersion(version);
        }

        /// <summary>
        /// Creates ABSaveSettings with control over the error handler, as well as the basic configuration.
        /// </summary>
        public ABSaveSettings(ABSaveErrorHandler errorHandler, bool withTypes = false, bool identifyTypes = true)
        {
            // Initialize all the basic parameters.
            InitializeBasicConfig(withTypes, identifyTypes);

            // Set the error handler.
            ErrorHandler = errorHandler;
        }

        /// <summary>
        /// Creates ABSaveSettings with a version, control over the error handler, as well as the basic configuration.
        /// </summary>
        public ABSaveSettings(int version, ABSaveErrorHandler errorHandler, bool withTypes = false, bool identifyTypes = true)
        {
            // Initialize all the basic parameters, and the version.
            InitializeBasicConfig(withTypes, identifyTypes);
            InitializeVersion(version);

            // Set the error handler.
            ErrorHandler = errorHandler;
        }

        /// <summary>
        /// Initializes ABSaveSettings with the two basic flags.
        /// </summary>
        private void InitializeBasicConfig(bool withTypes, bool identifyTypes = true)
        {
            // Set the two variables.
            WithTypes = withTypes;
            RememberTypes = identifyTypes;
        }

        private void InitializeVersion(int version)
        {
            // Mark us as having a version, and set the version.
            HasVersion = true;
            Version = version;
        }
        #endregion
    }
}
