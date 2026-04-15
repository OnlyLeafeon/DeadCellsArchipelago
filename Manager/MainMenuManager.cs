using System.Security.Cryptography;
using dc;
using dc.h3d.mat;
using dc.hl;
using dc.hxd;
using dc.pr;
using dc.ui;
using Hashlink.Virtuals;
using HaxeProxy.Runtime;
using ModCore.Utilities;
using Serilog;
using static DeadCellsArchipelago.ItemManager;
using static DeadCellsArchipelago.ImageManager;
using Newtonsoft.Json;

namespace DeadCellsArchipelago {
    public static class MainMenuManager
    {
        public static dc.h2d.Object? apMenuContainer = null;
        public static Text? connectionStatus = null;
        public static dc.h2d.TextInput? serverIp = null;
        public static dc.h2d.TextInput? slotName = null;
        public static dc.h2d.TextInput? password = null;
        public static Text? connectButton = null;

        public static TextInput? test = null;

        public static void OnUpdate(Hook_TitleScreen.orig_update orig, TitleScreen self)
        {
            /*double scale = 1;
            Text label = new Text(self.root, false, false, new Ref<double>(ref scale), null, null)
            {
                x = 0,
                y = 0
            };
            label.set_text("Mon texte ici".AsHaxeString());
            label.set_textColor(16711680);*/
            //label.draw();
            //Log.Warning($"{self.sizeFactorX} {self.sizeFactorY}");
            
            orig(self);
            if(test != null)
            {
                test.bgInput.set_visible(false);
            }
            /*self.padWarning.set_text("test".AsHaxeString());
            self.padWarning.x = 0;
            self.padWarning.y = 0;*/
            //Log.Warning("update !");
        }

        public static void OnMainMenu(Hook_TitleScreen.orig_mainMenu orig, TitleScreen self)
        {
            self.news.hidden = true;
            self.news.updateVisible();
            int menuScale = 3;
            if (apMenuContainer == null) {
                apMenuContainer = new dc.h2d.Object(self.root)
                {
                    x = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH * 0.7,
                    y = dc.libs.Process.Class.CUSTOM_STAGE_HEIGHT * 0.05,
                };

                int frame = 0;
                double XY = 0;
                dc.h2d.Tile bgTileApMenu = Assets.Class.ui.getTile("walterWhite".AsHaxeString(), new Ref<int>(ref frame), new Ref<double>(ref XY), new Ref<double>(ref XY), null);
                dc.h2d.Tile frameTileApMenu = Assets.Class.ui.getTile("boxLegendary".AsHaxeString(), new Ref<int>(ref frame), new Ref<double>(ref XY), new Ref<double>(ref XY), null);
                //string cwd = Directory.GetCurrentDirectory();
                //Log.Information($"CWD: {cwd}");
                //dc.h2d.Tile test = Res.Class.load.Invoke("coremod/mods/DeadCellsArchipelago/res/BestImageEver.png".AsHaxeString()).toTile();

                var tile = LoadTileFromPng(GetResPath("BestImageEver.png"));

                double R = 34.0/255;
                double G = 34.0/255;
                double B = 74.0/255;
                double A = 1;
                var bgApMenu = new dc.h2d.Bitmap(bgTileApMenu, apMenuContainer)
                {
                    scaleX = (113-4)*menuScale,
                    scaleY = (113-4)*menuScale,
                    x = 2*menuScale,
                    y = 2*menuScale,
                    color = new dc.h3d.Vector(new Ref<double>(ref R), new Ref<double>(ref G), new Ref<double>(ref B), new Ref<double>(ref A))
                };

                var apMenu = new dc.h2d.Bitmap(frameTileApMenu, apMenuContainer)
                {
                    scaleX = menuScale,
                    scaleY = menuScale
                };

                var myBitmap = new dc.h2d.Bitmap(tile, apMenuContainer)
                {
                    scaleX = 0.1,
                    scaleY = 0.1
                };
            }


            int index = 0;
            if (connectionStatus == null) {
                double scale = 1;
                connectionStatus = new Text(apMenuContainer, false, false, new Ref<double>(ref scale), null, null)
                {
                    x = 10,
                    y = 10+40*index
                };
                index++;

                connectionStatus.set_text("Not Connected".AsHaxeString());
                connectionStatus.set_textColor(16711680);
            }


            if (serverIp == null) {
                double scale = 1;
                Text serverIpTag = new Text(apMenuContainer, false, false, new Ref<double>(ref scale), null, null)
                {
                    y = 10+40*index
                };
                index++;

                serverIpTag.set_text("Address:".AsHaxeString());
                serverIpTag.x = ((113-4)*menuScale - serverIpTag.textWidth) /2;

                serverIpTag.set_textColor(16777215);


                UIBox bgServerIp = UIBox.Class.drawBoxMain(270, 1, 4, 3, 0, null);
                bgServerIp.x = 18;
                bgServerIp.y = 40*index;
                bgServerIp.alpha = 0.5;

                apMenuContainer.addChild(bgServerIp);

                serverIp = new dc.h2d.TextInput(connectionStatus.font, apMenuContainer)
                {
                    x = 18+6,
                    y = 2+40*index,
                    inputWidth = 284,
                    onMove = (e) =>
                    {
                        serverIp?.set_textColor(16776960);
                    },
                    onOut = (e) =>
                    {
                        serverIp?.set_textColor(16777215);
                    }
                };
                index++;
            }


            if (slotName == null) {
                double scale = 1;
                Text slotNameTag = new Text(apMenuContainer, false, false, new Ref<double>(ref scale), null, null)
                {
                    y = 10+40*index
                };
                index++;

                slotNameTag.set_text("Slot:".AsHaxeString());
                slotNameTag.x = ((113-4)*menuScale - slotNameTag.textWidth) /2;

                slotNameTag.set_textColor(16777215);


                UIBox bgSlotName = UIBox.Class.drawBoxMain(270, 1, 4, 3, 0, null);
                bgSlotName.x = 18;
                bgSlotName.y = 40*index;
                bgSlotName.alpha = 0.5;

                apMenuContainer.addChild(bgSlotName);

                slotName = new dc.h2d.TextInput(connectionStatus.font, apMenuContainer)
                {
                    x = 18+6,
                    y = 2+40*index,
                    inputWidth = 284,
                    onMove = (e) =>
                    {
                        slotName?.set_textColor(16776960);
                    },
                    onOut = (e) =>
                    {
                        slotName?.set_textColor(16777215);
                    }
                };
                index++;
                slotName.set_text("test".AsHaxeString());
            }


            if (password == null) {
                double scale = 1;
                Text passwordTag = new Text(apMenuContainer, false, false, new Ref<double>(ref scale), null, null)
                {
                    y = 10+40*index
                };
                index++;

                passwordTag.set_text("Password:".AsHaxeString());
                passwordTag.x = ((113-4)*menuScale - passwordTag.textWidth) /2;

                passwordTag.set_textColor(16777215);


                UIBox bgPassword = UIBox.Class.drawBoxMain(270, 1, 4, 3, 0, null);
                bgPassword.x = 18;
                bgPassword.y = 40*index;
                bgPassword.alpha = 0.5;

                apMenuContainer.addChild(bgPassword);

                password = new dc.h2d.TextInput(connectionStatus.font, apMenuContainer)
                {
                    x = 18+6,
                    y = 2+40*index,
                    inputWidth = 284,
                    onMove = (e) =>
                    {
                        password?.set_textColor(16776960);
                    },
                    onOut = (e) =>
                    {
                        password?.set_textColor(16777215);
                    }
                };
                index++;
                password.set_text("test".AsHaxeString());
            }

            if (connectButton == null) {
                double scale = 1;
                connectButton = new Text(apMenuContainer, false, true, new Ref<double>(ref scale), null, null)
                {
                    y = 40*index
                };
                index++;
                connectButton.set_text("Connect".AsHaxeString());
                connectButton.x = ((113-4)*menuScale - connectButton.textWidth) /2;
                connectButton.set_textColor(16777215);
                var inter = new dc.h2d.Interactive(
                    connectButton.get_textWidth(),
                    connectButton.get_textHeight(),
                    connectButton,
                    null
                )
                {
                    onClick = (e) =>
                    {
                        SaveFields();
                        var archipelago = new ArchipelagoManager();
                        archipelago.Connect(serverIp.text.ToString(), slotName.text.ToString(), password.text?.ToString());
                        ARCHIPELAGO = archipelago;
                        if (ARCHIPELAGO.IsConnected)
                        {
                            connectionStatus.set_text("Connected".AsHaxeString());
                            connectionStatus.set_textColor(2883371);
                        }
                        else
                        {
                            connectionStatus.set_text("Failed to Connect".AsHaxeString());
                            connectionStatus.set_textColor(16711680);
                        }
                    },
                    onMove = (e) =>
                    {
                        connectButton.set_textColor(16776960);
                    },
                    onOut = (e) =>
                    {
                        connectButton.set_textColor(16777215);
                    }
                };
            }

            SetValueFields();

            orig(self);

            //ModCore.Modules.Game


        }

        public static void OnOnResize(Hook_TitleScreen.orig_onResize orig, TitleScreen self)
        {
            orig(self);
            if (apMenuContainer != null)
            {
                apMenuContainer.x = dc.libs.Process.Class.CUSTOM_STAGE_WIDTH * 0.7;
                apMenuContainer.y = dc.libs.Process.Class.CUSTOM_STAGE_HEIGHT * 0.05;
            }
        }

        private static void SetValueFields()
        {
            ConfData confData = GetConfData();
            serverIp?.set_text(confData.serverIp.AsHaxeString());
            slotName?.set_text(confData.slotName.AsHaxeString());
            password?.set_text(confData.password?.AsHaxeString());
        }

        private static void SaveFields()
        {
            if (serverIp != null && slotName != null && password != null)
            {
                ConfData confData = new ConfData()
                {
                    serverIp = serverIp.text.ToString(),
                    slotName = slotName.text.ToString(),
                    password = password.text.ToString(),
                };
                SaveConfData(confData);
            }
        }

        public static string GetConfFilePath()
        {
            return Path.Combine(AppContext.BaseDirectory, "..", "..", "mods", "DeadCellsArchipelago", "data", "conf.json");
        }

        public static ConfData GetConfData()
        {
            var confPath = GetConfFilePath();
            var confData = new ConfData();
            if (System.IO.File.Exists(confPath))
            {
                var json = System.IO.File.ReadAllText(confPath);
                confData = JsonConvert.DeserializeObject<ConfData>(json) ?? new();
            }
            else
            {
                SaveConfData(confData);
            }
            return confData;
        }

        public static void SaveConfData(ConfData confData)
        {
            var json = JsonConvert.SerializeObject(confData, Formatting.Indented);
            System.IO.File.WriteAllText(GetConfFilePath(), json);
        }
    }
}