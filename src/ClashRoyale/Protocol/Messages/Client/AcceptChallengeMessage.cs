﻿using System;
using ClashRoyale.Logic;
using DotNetty.Buffers;

namespace ClashRoyale.Protocol.Messages.Client
{
    public class AcceptChallengeMessage : PiranhaMessage
    {
        public AcceptChallengeMessage(Device device, IByteBuffer buffer) : base(device, buffer)
        {
            Id = 14120;
        }

        public long EntryId { get; set; }

        public override void Decode()
        {
            EntryId = Reader.ReadLong();
        }

        public override async void Process()
        {
            var home = Device.Player.Home;
            var alliance = await Resources.Alliances.GetAllianceAsync(home.AllianceInfo.Id);

            var entry = alliance?.Stream.Find(e => e.Id == EntryId);

            if (entry != null)
            {
                alliance.RemoveEntry(entry);

                var enemy = await Resources.Players.GetPlayerAsync(entry.SenderId);

                if (enemy.Device != null)
                {
                    var battle = new Battle(false)
                    {
                        Device.Player, enemy
                    };

                    Device.Player.Battle = battle;
                    enemy.Battle = battle;

                    battle.Start();
                }

                alliance.Save();

                // TODO: Update Entry + Battle Result + Card levels
            }
        }
    }
}