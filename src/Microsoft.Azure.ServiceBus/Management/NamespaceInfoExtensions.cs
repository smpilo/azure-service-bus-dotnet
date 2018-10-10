﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.ServiceBus.Management
{
    using System;
    using System.Xml.Linq;

    internal class NamespaceInfoExtensions
    {
        public static NamespaceInfo ParseFromContent(string xml)
        {
            try
            {
                var xDoc = XElement.Parse(xml);
                if (!xDoc.IsEmpty)
                {
                    if (xDoc.Name.LocalName == "entry")
                    {
                        return ParseFromEntryElement(xDoc);
                    }
                }
            }
            catch (Exception ex) when (!(ex is ServiceBusException))
            {
                throw new ServiceBusException(false, ex);
            }

            throw new ServiceBusException(false, "Unknown error.");
        }

        private static NamespaceInfo ParseFromEntryElement(XElement xEntry)
        {
            var nsInfo = new NamespaceInfo();

            var nsInfoXml = xEntry.Element(XName.Get("content", ManagementClientConstants.AtomNs))?
                .Element(XName.Get("NamespaceInfo", ManagementClientConstants.SbNs));

            if (nsInfoXml == null)
            {
                throw new ServiceBusException(true);
            }

            foreach (var element in nsInfoXml.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "CreatedTime":
                        nsInfo.CreatedTime = DateTime.Parse(element.Value);
                        break;
                    case "ModifiedTime":
                        nsInfo.ModifiedTime = DateTime.Parse(element.Value);
                        break;
                    case "Name":
                        nsInfo.Name = element.Value;
                        break;
                    case "NamespaceType":
                        if (Enum.TryParse<NamespaceType>(element.Value, out var nsType))
                        {
                            nsInfo.NamespaceType = nsType;
                        }
                        else
                        {
                            nsInfo.NamespaceType = NamespaceType.Unknown;
                        }
                        break;
                    case "MessagingSKU":
                        if (Enum.TryParse<MessagingSku>(element.Value, out var nsSku))
                        {
                            nsInfo.MessagingSku = nsSku;
                        }
                        break;
                }
            }

            return nsInfo;
        }
    }
}
