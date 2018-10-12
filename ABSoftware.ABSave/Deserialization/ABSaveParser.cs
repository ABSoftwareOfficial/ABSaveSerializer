using ABParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABSoftware.ABSave.Deserialization
{
    /// <summary>
    /// Parses ABSave - powered by ABParser.
    /// </summary>
    public class ABSaveParser : ABParser
    {
        /// <summary>
        /// All of the objects/arrays/dictionaries we're currently in.
        /// </summary>
        public object[] InnerLevel;

        /// <summary>
        /// Used in the unnamed ABSave - keeps track of which item we're on (to keep things in order).
        /// </summary>
        public int CurrentItem;

        /// <summary>
        /// Whether we've established the header.
        /// </summary>
        public bool EstablishedHeader = false;

        /// <summary>
        /// PROTOTYPE! Creates a new ABSaveParser with all the correct configuration.
        /// </summary>
        public ABSaveParser()
        {
            // Set all the tokens for the ABSave.
            Tokens = new System.Collections.ObjectModel.ObservableCollection<ABParserToken>()
            {
                new ABParserToken("NULL", '\u0000'),
                new ABParserToken("NEXT ITEM", '\u0001'),
                new ABParserToken("START OBJECT", '\u0003'),
                new ABParserToken("START ARRAY", '\u0004'),
                new ABParserToken("LOWER INNERLEVEL", '\u0005'),
                new ABParserToken("START DICTIONARY", '\u0006'),
            };
        }

        protected override void OnStart()
        {
            // Run all the default code to allow ABParser to run.
            base.OnStart();

            // Now, we want to make it tell us about the first character, since that's the header.
           
        }

        protected override void OnTokenProcessed(TokenProcessedEventArgs e)
        {
            // Run all the default code to allow ABParser to run.
            base.OnTokenProcessed(e);

            // Run the correct code based on what the token is.
            switch (e.Token.Name)
            {
                case "NULL":

                    break;
            }
        }
    }
}
