﻿/*
Copyright (c) 2013 <a href="http://www.gutgames.com">James Craig</a>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation Configs (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.*/

#region Usings
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Utilities.Configuration.Manager.Interfaces;
using Utilities.DataTypes;
using System.Linq;
using System.Reflection;
using Utilities.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization;
#endregion

namespace Utilities.Configuration.Manager.BaseClasses
{
    /// <summary>
    /// Default config base class
    /// </summary>
    /// <typeparam name="ConfigClassType">Config class type</typeparam>
    [Serializable]
    public abstract class Config<ConfigClassType> : Dynamo<ConfigClassType>, IConfig
        where ConfigClassType : Config<ConfigClassType>, new()
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="StringToObject">String to object</param>
        /// <param name="ObjectToString">Object to string</param>
        protected Config(Func<string, ConfigClassType> StringToObject = null, Func<ConfigClassType, string> ObjectToString = null)
        {
            this.ObjectToString = ObjectToString.Check(x => x.Serialize<string, ConfigClassType>(SerializationType.XML));
            this.StringToObject = StringToObject.Check(x => x.Deserialize<ConfigClassType, string>(SerializationType.XML));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Location to save/load the config file from.
        /// If blank, it does not save/load but uses any defaults specified.
        /// </summary>
        protected virtual string ConfigFileLocation { get { return ""; } }

        /// <summary>
        /// Encryption password for properties/fields. Used only if set.
        /// </summary>
        protected virtual string EncryptionPassword { get { return ""; } }

        /// <summary>
        /// Name of the Config object
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the object
        /// </summary>
        private Func<string, ConfigClassType> StringToObject { get; set; }

        /// <summary>
        /// Gets a string representation of the object
        /// </summary>
        private Func<ConfigClassType, string> ObjectToString { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Loads the config
        /// </summary>
        public void Load()
        {
            if (string.IsNullOrEmpty(ConfigFileLocation))
                return;
            string FileContent = new FileInfo(ConfigFileLocation).Read();
            if (string.IsNullOrEmpty(FileContent))
            {
                Save();
                return;
            }
            LoadProperties(StringToObject(FileContent));
            Decrypt();
        }

        /// <summary>
        /// Saves the config
        /// </summary>
        public void Save()
        {
            if (string.IsNullOrEmpty(ConfigFileLocation))
                return;
            Encrypt();
            new FileInfo(ConfigFileLocation).Write(ObjectToString((ConfigClassType)this));
            Decrypt();
        }

        private void LoadProperties(ConfigClassType Temp)
        {
            if (Temp == null)
                return;
            foreach (KeyValuePair<string, object> Item in this)
            {
                SetValue(Item.Key, Item.Value);
            }
        }

        private void Encrypt()
        {
            if (string.IsNullOrEmpty(EncryptionPassword))
                return;
            using (PasswordDeriveBytes Temp = new PasswordDeriveBytes(EncryptionPassword, "Kosher".ToByteArray(), "SHA1", 2))
            {
                foreach (KeyValuePair<string, object> Item in this.Where(x => x.Value.GetType() == typeof(string)))
                {
                    SetValue(Item.Key, ((string)Item.Value).Encrypt(Temp));
                }
            }
        }

        private void Decrypt()
        {
            if (string.IsNullOrEmpty(EncryptionPassword))
                return;
            using (PasswordDeriveBytes Temp = new PasswordDeriveBytes(EncryptionPassword, "Kosher".ToByteArray(), "SHA1", 2))
            {
                foreach (KeyValuePair<string, object> Item in this.Where(x => x.Value.GetType() == typeof(string)))
                {
                    SetValue(Item.Key, ((string)Item.Value).Decrypt(Temp));
                }
            }
        }

        #endregion
    }
}