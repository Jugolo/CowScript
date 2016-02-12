﻿using script.parser;
using script.token;
using script.variabel;

namespace script.stack
{
    class AgumentParser
    {
        private static bool hasDefault = false;

        public static AgumentStack parseAguments(Token token, VariabelDatabase database, EnegyData data)
        {
            hasDefault = false;//be sure :)
            AgumentStack agument = new AgumentStack();
            if(token.getCache().type() != TokenType.LeftBue)
            {
                data.setError(new ScriptError("Excpect ( got: " + token.getCache().ToString(), token.getCache().posision()), database);
                return null;
            }

            //control if wee need to look and parse aguments :)
            if(token.next().type() != TokenType.RightBue)
            {
                //wee need :)
                if (!getSingleAguments(token, agument, database, data))
                    return new AgumentStack();
                while(token.getCache().type() == TokenType.Comma)
                {
                    token.next();
                    if (!getSingleAguments(token, agument, database, data))
                        return new AgumentStack();
                }
            }

            //control wee got to ) 
            if(token.getCache().type() != TokenType.RightBue)
            {
                data.setError(new ScriptError("Missing ) after function aguments got: " + token.getCache().ToString(), token.getCache().posision()), database);
            }

            return agument;
        }

        private static bool getSingleAguments(Token token, AgumentStack agument, VariabelDatabase database, EnegyData data)
        {   string type = null;
            string name = null;
            CVar value = null;

            //okay wee got a variabel. control if it is a type :)
            if (database.isType(token.getCache().ToString()))
            {
                //yes it is a type :)
                type = token.getCache().ToString();

                //okay let try to find the name 
                if(token.next().type() != TokenType.Variabel)
                {
                    data.setError(new ScriptError("After type there must be a type", token.getCache().posision()), database);
                    return false;
                }

                name = token.getCache().ToString();
            }
            else
            {
                if (token.getCache().type() != TokenType.Variabel)
                {
                    data.setError(new ScriptError("Unknown agument token: " + token.getCache().ToString(), token.getCache().posision()), database);
                    return false;
                }
                name = token.getCache().ToString();
            }

            if (token.next().type() == TokenType.Assigen)
            {
                token.next();
                value = new VariabelParser().parseNoEnd(data, database, token);
                hasDefault = true;
            }
            else
            {
                if (hasDefault)
                {
                    data.setError(new ScriptError("You can not put non default after a defualt variabel!", token.getCache().posision()), database);
                    return false;
                }
            }

            agument.push(type, name, value);
            return true;
        }
    }
}
