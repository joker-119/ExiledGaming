using System;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using UnityEngine;

namespace ExiledGaming.Commands.Hats
{
    using Exiled.API.Enums;

    public class Give : ICommand
    {
        public string Command { get; } = "give";
        public string[] Aliases { get; } = new[] { "g" };
        public string Description { get; } = "Gives a player a hat.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("jp.hats.give"))
            {
                response = "You are not permitted to run this command.";
                return false;
            }
            
            Player player = Player.Get(((CommandSender)sender).SenderId);

            Player target;
            ItemType type;

            if (arguments.Count < 2)
            {
                response = "You must define a player to give the hat to, and an ItemType of the hat.";
                return false;
            }
            
            target = Player.Get(arguments.At(0));
            if (target == null)
            {
                response = $"Unable to find player '{arguments.At(0)}'";
                return false;
            }

            try
            {
                type = (ItemType) Enum.Parse(typeof(ItemType), arguments.At(1));
            }
            catch (Exception)
            {
                response = $"Unable to parse '{arguments.At(1)}' as a valid item.";
                return false;
            }

            Vector3 scale = Vector3.zero;
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.Euler(0,0,0);
            
            if (arguments.Count > 2)
            {
                if (arguments.Count < 5)
                {
                    response = "If you are defining a scale, you must define all 3 scale axis (x, y and z).";
                    return false;
                }

                if (!float.TryParse(arguments.At(2), out float x))
                {
                    response = $"{arguments.At(2)} is not a valid numerical axis value for X.";
                    return false;
                }

                if (!float.TryParse(arguments.At(3), out float y))
                {
                    response = $"{arguments.At(3)} is not a valid numerical axis value for Y.";
                    return false;
                }

                if (!float.TryParse(arguments.At(4), out float z))
                {
                    response = $"{arguments.At(4)} is not a valid numerical axis value for Z.";
                    return false;
                }

                scale = new Vector3(x, y, z);

                if (arguments.Count > 5)
                {
                    if (arguments.Count < 8)
                    {
                        response =
                            "If you are defining a custom position, you must define all 3 axis locations (x, y and z).";
                        return false;
                    }
                    
                    if (!float.TryParse(arguments.At(5), out float pX))
                    {
                        response = $"{arguments.At(5)} is not a valid numerical axis value for X.";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(6), out float pY))
                    {
                        response = $"{arguments.At(6)} is not a valid numerical axis value for Y.";
                        return false;
                    }

                    if (!float.TryParse(arguments.At(7), out float pZ))
                    {
                        response = $"{arguments.At(7)} is not a valid numerical axis value for Z.";
                        return false;
                    }

                    pos = new Vector3(pX, pY, pZ);

                    if (arguments.Count > 8)
                    {
                        if (arguments.Count < 11)
                        {
                            response =
                                "If you are defining a custom rotation, you must define all 3 rotational axis values (x, y and z).";
                            return false;
                        }
                        
                        if (!float.TryParse(arguments.At(8), out float rX))
                        {
                            response = $"{arguments.At(8)} is not a valid numerical axis value for X.";
                            return false;
                        }

                        if (!float.TryParse(arguments.At(9), out float rY))
                        {
                            response = $"{arguments.At(9)} is not a valid numerical axis value for Y.";
                            return false;
                        }

                        if (!float.TryParse(arguments.At(10), out float rZ))
                        {
                            response = $"{arguments.At(10)} is not a valid numerical axis value for Z.";
                            return false;
                        }
                        
                        rot = Quaternion.Euler(rX, rY, rZ);
                    }
                }
            }

            HatInfo info = new HatInfo(type, scale, pos, rot);
            
            target.SpawnHat(info);

            response = $"{target.Nickname} has been given a hat! {info.Item}";
            return true;
        }
    }
}