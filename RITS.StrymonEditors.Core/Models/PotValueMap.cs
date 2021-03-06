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
    /// List of <see cref="PotValueItem"/> instances used as a 'map'
    /// to handle the 'curve' of the pot control in the UI
    /// </summary>
    [DataContract, XmlSerializerFormat]
    public class PotValueMap : List<PotValueItem>
    {
        private List<PotValueItem> expanded;
        

        /// <summary>
        /// Exposes the expanded list for Lookup purposes
        /// </summary>
        public List<PotValueItem> LookupMap 
        { 
            get 
            {
                if (expanded == null) ExpandList();
                return expanded; 
            } 
        }

        /// <summary>
        /// Retrieves the correct value for a supplied angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public int GetValueForAngle(double angle)
        {
            var map = LookupMap.FirstOrDefault(x => x.Angle > angle);
            if (map.Value == 0) return map.Value;
            var prev = LookupMap.FirstOrDefault(x => x.Value == map.Value - 1);
            return prev.Value;

        }

        /// <summary>
        /// Retrieves the correct angle for a specified value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double GetAngleForValue(int value)
        {
            var map = LookupMap.FirstOrDefault(x => x.Value == value);
            if (map != null)
            {
                return map.Angle;
            }
            return 0;
        }

        /// <summary>
        /// Resets the expanded list back to null, to force a reload
        /// </summary>
        public void Reset()
        {
            expanded = null;

        }

        /// <summary>
        /// Applies an increment map - or list of <see cref="Increment"/> to the supplied definition
        /// to populate the list of 128 possible values that are possible for a <see cref="Pot"/> 
        /// with the correct 'fine' value
        /// This method is currently only relevant to the fine/coarse pots and parameters, 
        /// allowing different machines to correctly be synchronised with the actual pedal
        /// </summary>
        /// <param name="incrementMap"></param>
        /// <param name="definition"></param>
        public void ApplyFineValueIncrementMap(List<Increment> incrementMap, ParameterDef definition)
        {
            var range = definition.CoarseRange == null ? definition.FineRange : definition.CoarseRange;

            int iAltIndex = 0;
            int fineValueAccum = range.MinValue;
            var incItem = incrementMap[0];

            foreach (var pvi in LookupMap)
            {
                if (pvi.Value == 0) { pvi.FineValue = range.MinValue; }
                else
                {

                    if (incItem.End == 0) incItem.End = range.MaxValue;
                    int actualIncValue = incItem.GetIncrementValue(ref iAltIndex);
                    iAltIndex++;
                    // Assumption here is that increment map works out perfectly
                    fineValueAccum += actualIncValue;
                    if (fineValueAccum > incItem.End)fineValueAccum=incItem.End;
                    pvi.FineValue = fineValueAccum;
                    if (fineValueAccum == incItem.End)
                    {
                        // Need to 
                        incItem = incrementMap.FirstOrDefault(x => x.End > fineValueAccum);
                        if (incItem == null)
                        {
                            incItem = incrementMap.Last();
                        }
                        //Reset alt index
                        iAltIndex = 0;
                    }

                }
                if (fineValueAccum >= range.MaxValue)
                {
                    break;
                }
            }
        }

        // Expands the configured list of items into a full list of 128 items 
        // extrapolating angle/values between the supplied configured values
        private void ExpandList()
        {
            expanded = new List<PotValueItem>();
            PotValueItem prev = null;
            foreach (var m in this.OrderBy(x => x.Value))
            {
                if (prev != null)
                {
                    Expand(prev, m);
                }
                prev = m;
            }
            expanded.Add(prev);

        }

        // Expands between the supplied from and to items
        private void Expand(PotValueItem from, PotValueItem to)
        {
            expanded.Add(new PotValueItem { Value = from.Value, FineValue = from.FineValue, Angle = from.Angle, ClockPosition = from.ClockPosition });

            int noOfValues = (to.Value - (from.Value + 1));
            double angleDelta = to.Angle - from.Angle;
            double angleIncrement = angleDelta / (noOfValues + 1);
            for (int i = 1; i <= noOfValues; i++)
            {
                expanded.Add(new PotValueItem { Value = from.Value + i, Angle = from.Angle + (angleIncrement * i) });
            }
        }
    }
}
