using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AllEnum;
using System.IO;
using System.Net;

namespace PokemonGo.RocketAPI.Window
{
    public partial class PokemonList : Form
    {

        private DataTable allPkm = new DataTable();
        public PokemonList()
        {
            InitializeComponent();

            InitListView();
            LoadList();
        }

        private void InitListView()
        {
            pkmList.RowTemplate.Height = ReadSettings.imageSize;

            var colId = new DataColumn();
            colId.DataType = typeof(int);
            colId.ColumnName = "Id";
            allPkm.Columns.Add(colId);

            var colPkm = new DataColumn();
            colPkm.DataType = typeof(string);
            colPkm.ColumnName = "Pokémon";
            allPkm.Columns.Add(colPkm);

            var colEvolve = new DataColumn();
            colEvolve.DataType = typeof(bool);
            colEvolve.ColumnName = "Evolve";
            allPkm.Columns.Add(colEvolve);

            var colTransfer = new DataColumn();
            colTransfer.DataType = typeof(bool);
            colTransfer.ColumnName = "Transfer";
            allPkm.Columns.Add(colTransfer);

            var colCatch = new DataColumn();
            colCatch.DataType = typeof(bool);
            colCatch.ColumnName = "Catch";
            allPkm.Columns.Add(colCatch);


            var colImg = new DataColumn();
            colImg.DataType = typeof(Image);
            colImg.ColumnName = " ";
            allPkm.Columns.Add(colImg);

            if (ReadSettings.poke != "")
            {
                StringReader sr = new StringReader(ReadSettings.poke);
                allPkm.TableName = "Poke";
                allPkm.ReadXml(sr);
            }


            allPkm.Columns["Id"].SetOrdinal(0);
            allPkm.Columns[" "].SetOrdinal(1);
            allPkm.Columns["Pokémon"].SetOrdinal(2);
            allPkm.Columns["Evolve"].SetOrdinal(3);
            allPkm.Columns["Transfer"].SetOrdinal(4);
            allPkm.Columns["Catch"].SetOrdinal(5);

            pkmList.DataSource = allPkm;
        }

        private void LoadList()
        {
            List<string> pokeName;

            switch(ReadSettings.language)
            {
                case "en":
                    pokeName = LanguageSetting.englishPokemons;
                    break;

                case "de":
                    pokeName = LanguageSetting.GermanName;
                    break;

                case "fr":
                    pokeName = LanguageSetting.frenchPokemons;
                    break;

                default:
                    pokeName = LanguageSetting.englishPokemons;
                    break;
            }

            foreach(PokemonId id in Enum.GetValues(typeof(PokemonId)))
            {
                if (id == 0) continue;
                if (ReadSettings.poke == "")
                {
                    DataRow dtRow = allPkm.NewRow();

                    dtRow[1] = ScaleImage(GetPokemonImage((int)id), ReadSettings.imageSize, ReadSettings.imageSize);

                    dtRow[0] = (int)id;
                    dtRow[2] = pokeName.ElementAt((int)id);
                    dtRow[3] = CatchesEvolveTransfersSettings.toEvolve.Contains(id);
                    dtRow[4] = CatchesEvolveTransfersSettings.toTransfert.Contains(id);
                    dtRow[5] = !CatchesEvolveTransfersSettings.toNotCatch.Contains(id);
                    allPkm.Rows.Add(dtRow);
                }
                else
                {
                    allPkm.Rows[(int)id - 1][1] = ScaleImage(GetPokemonImage((int)id), ReadSettings.imageSize, ReadSettings.imageSize);
                }            
            }
           // SaveOnVariables();

            pkmList.Columns[0].Visible = false;
            pkmList.Columns[1].Width = ReadSettings.imageSize;
            pkmList.Columns[1].ReadOnly = true;
            pkmList.Columns[2].ReadOnly = true;
            
        }
        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }

        private Image GetPokemonImage(int pokemonId)
        {
            var Sprites = AppDomain.CurrentDomain.BaseDirectory + "Sprites\\";
            string location = Sprites + pokemonId + ".png";
            if (!Directory.Exists(Sprites))
                Directory.CreateDirectory(Sprites);
            if (!File.Exists(location))
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("http://pokeapi.co/media/sprites/pokemon/" + pokemonId + ".png", @location);
            }
            return Image.FromFile(location);
        }

        private void pkmList_SelectionChanged(object sender, EventArgs e)
        {
            pkmList.ClearSelection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveOnVariables();
            StringWriter sw = new StringWriter();
            allPkm.TableName = "Poke";
            allPkm.Columns.RemoveAt(1);
            allPkm.WriteXml(sw);
            ReadSettings.poke = sw.ToString();
            Settings.Instance.SetSetting(sw.ToString(), "Poke");
            sw.Dispose();
            Close();
        }

        private void SaveOnVariables()
        {
            CatchesEvolveTransfersSettings.toEvolve.Clear();
            CatchesEvolveTransfersSettings.toTransfert.Clear();
            CatchesEvolveTransfersSettings.toNotCatch.Clear();
            foreach (DataRow pkm in allPkm.Rows)
            {
                if ((bool)pkm[3])
                {
                    CatchesEvolveTransfersSettings.toEvolve.Add((PokemonId)((int)pkm[0]));
                }

                if ((bool)pkm[4])
                {
                    CatchesEvolveTransfersSettings.toTransfert.Add((PokemonId)((int)pkm[0]));
                }

                if (!(bool)pkm[5])
                {
                    CatchesEvolveTransfersSettings.toNotCatch.Add((PokemonId)((int)pkm[0]));
                }
            }
        }


    }
}
