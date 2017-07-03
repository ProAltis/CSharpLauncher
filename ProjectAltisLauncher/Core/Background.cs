﻿using System;
using System.Drawing;

namespace ProjectAltis.Core
{
    public static class Background
    {
        private static readonly Random rand = new Random();

        public static Image ReturnRandomBackground()
        {
            int val = rand.Next(1, 7); // Generates a random number 1-6
            switch (val)
            {
                case 1:
                    return ReturnBackground("TTC");
                case 2:
                    return ReturnBackground("DD");
                case 3:
                    return ReturnBackground("DG");
                case 4:
                    return ReturnBackground("MML");
                case 5:
                    return ReturnBackground("Brrrgh");
                case 6:
                    return ReturnBackground("DDLs");
            }
            return ReturnBackground("TTC"); // Return a default case of TTC, is this possible?
        }

        public static Image ReturnBackground(string bg)
        {
            switch (bg)
            {
                case "TTC":
                    return Properties.Resources.TTC;
                case "DD":
                    return Properties.Resources.DD;
                case "DG":
                    return Properties.Resources.DG;
                case "MML":
                    return Properties.Resources.MML;
                case "Brrrgh":
                    return Properties.Resources.Brrrgh;
                case "DDL":
                    return Properties.Resources.DDL;
				default:
					return Properties.Resources.TTC;
            }
        }

        public static Image ImageChooser(string name, string method)
        {
            method = method.ToLower();
            switch (name.ToLower())
            {
                case "create":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.create_h;
                        case "mouseleave":
                            return Properties.Resources.create;
                        case "mousedown":
                            return Properties.Resources.create_d;
                        case "mouseup":
                            return Properties.Resources.create;
                    }
                    break;
                case "website":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.website_h;
                        case "mouseleave":
                            return Properties.Resources.website;
                        case "mousedown":
                            return Properties.Resources.website_d;
                        case "mouseup":
                            return Properties.Resources.website;
                    }
                    break;
                case "discord":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.discord_h;
                        case "mouseleave":
                            return Properties.Resources.discord;
                        case "mousedown":
                            return Properties.Resources.discord_d;
                        case "mouseup":
                            return Properties.Resources.discord;
                    }
                    break;
                case "group":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.group_h;
                        case "mouseleave":
                            return Properties.Resources.group;
                        case "mousedown":
                            return Properties.Resources.group_d;
                        case "mouseup":
                            return Properties.Resources.group;
                    }
                    break;
                case "theme":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.theme_h;
                        case "mouseleave":
                            return Properties.Resources.theme;
                        case "mousedown":
                            return Properties.Resources.theme_d;
                        case "mouseup":
                            return Properties.Resources.theme;
                    }
                    break;
                case "options":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.options_h;
                        case "mouseleave":
                            return Properties.Resources.options;
                        case "mousedown":
                            return Properties.Resources.options_d;
                        case "mouseup":
                            return Properties.Resources.options;
                    }
                    break;
                case "credits":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.credits_h;
                        case "mouseleave":
                            return Properties.Resources.credits;
                        case "mousedown":
                            return Properties.Resources.credits_d;
                        case "mouseup":
                            return Properties.Resources.credits;
                    }
                    break;
                case "play":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.play_h;
                        case "mouseleave":
                            return Properties.Resources.play;
                        case "mousedown":
                            return Properties.Resources.play_d;
                        case "mouseup":
                            return Properties.Resources.play;
                    }
                    break;
                case "exit":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.cancel_h;
                        case "mouseleave":
                            return Properties.Resources.cancel;
                    }
                    break;
                case "min":
                    switch (method)
                    {
                        case "mouseenter":
                            return Properties.Resources.minus_h;
                        case "mouseleave":
                            return Properties.Resources.minus;
                    }
                    break;
                case "contentpacks":
                    switch(method)
                    {
                        case "mouseenter":
                            return Properties.Resources.contentpacks_h;
                        case "mouseleave":
                            return Properties.Resources.contentpacks;
                        case "mousedown":
                            return Properties.Resources.contentpacks_d;
                        case "mouseup":
                            return Properties.Resources.contentpacks;
                    }
                    break;
            }
            return null;
        }
    }
}
