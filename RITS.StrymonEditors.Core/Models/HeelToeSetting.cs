﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ServiceModel;
namespace RITS.StrymonEditor.Models
{
    /// <summary>
    /// Defines an EPSet heel and toe setting for a specific <see cref="Pot"/>
    /// </summary>
    [DataContract, XmlSerializerFormat]
    public class HeelToeSetting
    {
        /// <summary>
        /// The Id of the relevant <see cref="Pot"/>
        /// </summary>
        [XmlAttribute]
        public int PotId { get; set; }

        /// <summary>
        /// The value for the heel position
        /// </summary>
        [XmlAttribute]
        public int HeelValue { get; set; }

        /// <summary>
        /// The value fro the toe position
        /// </summary>
        [XmlAttribute]
        public int ToeValue { get; set; }

    }
}
