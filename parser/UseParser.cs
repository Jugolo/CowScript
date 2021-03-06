﻿using script.plugin.File;
using script.token;
using script.variabel;
using System;
using System.IO;

namespace script.parser
{
    class UseParser : ParserInterface
    {
        public CVar parse(EnegyData ed, VariabelDatabase db, Token token)
        {
            token.next();

            includePlugin(new VariabelParser().parseNoEnd(ed, db, token).toString(token.getCache().posision(), ed, db), ed, db, token);

            while (token.getCache().type() == TokenType.Comma)
            {
                token.next();
                includePlugin(new VariabelParser().parseNoEnd(ed, db, token).toString(token.getCache().posision(), ed, db), ed, db, token);
            }

            if(token.getCache().type() != TokenType.End)
            {
                ed.setError(new ScriptError("Missing ; in end of use. got: "+token.getCache().ToString(), token.getCache().posision()), db);
                return null;
            }

            token.next();

            return new NullVariabel();
        }

        private void includePlugin(string name, EnegyData ed, VariabelDatabase db, Token token)
        {
            //control if the plugin exists in the system
            if (ed.Plugin.exists(name))
            {
                //wee has the plugin and load it :)
                ed.Plugin.open(db, name, ed, token.getCache().posision());
            }
            else
            {
                if (ed.Config.get("file.enabled", "false") == "false")
                {
                    ed.setError(new ScriptError("It is not allow to use file in use. 'file.enabled' is not set.", token.getCache().posision()), db);
                    return;
                }

                parseFile(ed, db, token.getCache().posision(), name);
            }
        }

        private void parseFile(EnegyData ed, VariabelDatabase db, Posision pos, string plugin)
        {
            if (!File.Exists(plugin))
            {
                ed.setError(new ScriptError("Unknown file: " + plugin, pos), db);
                return;
            }

            FileEnergy.parse(ed, new FileVariabelDatabase(db), plugin);
        }
    }
}
