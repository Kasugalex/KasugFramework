using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor.AssetBundleTools;

/*
Usage: TexturePacker [options] [<*.png|gif|tif|jpg|swf|...>] [<imagefolder>] [<*.tps>*]

You can specify one or more .png or .tps files or directories for processing.
   <folder>                     Recursively adds all known files in the sprite sheet
   <*.tps>                      *.tps file created with TexturePackerPro (requires pro license)
                                Additional set options override settings in the *.tps file
   <*.swf>                      Flash requires pro license
   <images>                     Image formats, supported:
                                     bmp       Bitmap
                                     gif       Graphics Interchange Format
                                     ico       Icon (Windows)
                                     jpeg      Joint Photographic Experts Group
                                     jpg       Joint Photographic Experts Group
                                     pbm       Portable Bitmap
                                     pgm       Portable Gray Map
                                     pkm       PKM file format (etc1)
                                     png       Portable Network Graphics
                                     ppm       Netpbm color image format
                                     psd       Photoshop image format
                                     pvr       PowerVR image format
                                     pvr.ccz   PowerVR image format (zlib)
                                     pvr.gz    PowerVR image format (gz)
                                     pvrtc     PowerVR image format
                                     svg       Scalable Vector Graphics
                                     svgz      Scalable Vector Graphics
                                     swf       Flash
                                     tga       Targa image format
                                     tif       Tagged Image Format
                                     tiff      Tagged Image Format
                                     webp      WebP format
                                     xbm       X Bitmap
                                     xpm       X Pixmap

Options:
  --help                        Display help text
  --version                     Print version information
  --gui                         Launch in with graphical user interface

  Output:
  --sheet <filename>            Name of the sheet to write, see texture-format for formats available
  --texture-format <id>         Sets the format for the textures.
                                The format type is automatically derived from the sheet's file name
                                if possible.
                                Available formats:
                                    png               PNG (32bit)
                                    png8              PNG (8bit indexed)
                                    pvr3              PowerVR Texture Format, PVR Version 3
                                    pvr3gz            PowerVR Texture Format, PVR Version 3, compressed with gzip
                                    pvr3ccz           PowerVR Texture Format, PVR Version 3, compressed with zlib, cocos2d header
                                    jpg               JPG image format, lossy compression, no transparency
                                    bmp               24 bit BMP
                                    tga               Targa Image Format
                                    tiff              Tagged Image File Format
                                    pkm               PKM image format, ETC1 compression
                                    webp              WebP lossless / lossy format
                                    atf               Adobe Texture Format
                                    pvr2              PowerVR Texture Format, PVR Version 2, deprecated
                                    pvr2gz            PowerVR Texture Format, PVR Version 2, compressed with gzip, deprecated
                                    pvr2ccz           PowerVR Texture Format, PVR Version 2, compressed with zlib, cocos2d header, deprecated

  --format <format>             Format to write, default is cocos2d
                                Available formats:
                                    2dtoolkit           2D Toolkit exporter
                                    agk                 Format for AppGameKit
                                    batterytech         BatteryTech Exporter
                                    bhive               Format for BHive
                                    caat                Exporter for CAAT - the Canvas Advanced Animation Toolkit
                                    cegui               Format for CEGUI / OGRE
                                    cocos2d             plist format version 3 for cocos2d
                                    cocos2d-v2          old plist format version 2 for cocos2d (deprecated)
                                    cocos2d-x           plist format version 3 for cocos2d-x with polygon packing
                                    corona-imagesheet   Exporter for Corona(TM) SDK using new image sheet format.
                                    css                 Creates CSS sprite sheets, responsive, with retina/highdpi support
                                    css-simple          Simple CSS sprite format for web design
                                    easeljs             Exporter for EaselJS.
                                    facebook            Facebook AR Studio
                                    gideros             Format for Gideros
                                    godot3-spritesheet  Godot 3 SpriteSheet - requires importer
                                    godot3-tileset      Godot 3 TileSet - requires importer
                                    json-array          Text file for json/html as array
                                    json                Text file for json/html as hash
                                    kwik                Exporter for Kwik using new image sheet format.
                                    less                Creates a LESS file that can be incorporated into a sprites arrangement
                                    libgdx              text file for lib GDX
                                    libRocket           Exporter to demonstrate how to crate your own exporters
                                    melonjs             Data file for MelonJS
                                    moai                Format for Moai
                                    molecule            Exporter for Molecule Framework
                                    monogame            Input format for the MonoGame TexturePacker Importer
                                    orx                 Orx Exporter
                                    panda               Exporter for Panda 2
                                    phaser-json-array   JSON array data for Phaser
                                    phaser-json-hash    JSON hash data for Phaser
                                    phaser              JSON format for Phaser 3
                                    pixijs              Data file for PixiJS
                                    sass-mixins         Exporter for SASS.
                                    shiva3d-jpsprite    Shiva3D with JPSprite extension
                                    shiva3d             Exporter for Shiva3D.
                                    slick2d             Format for Slick2D
                                    sparrow             xml file for Sparrow/Starling SDK
                                    spine               text file for Spine
                                    spritesheet-only    Exports only the sprite sheet without data file
                                    spritekit           plist format for SpriteKit, Objective-C header file
                                    spritekit-swift     plist format for SpriteKit, with swift class file
                                    spriter             JSON file for Spriter
                                    spritestudio        OPTPiX SpriteStudio 5 CellMap File.
                                    tresensa            Exporter for TreSensa TGE.
                                    uikit               Exporter for UIKit
                                    unity               Text file for Unity(R), json format with .txt ending
                                    unity-texture2d     Input format for the Unity(R) TexturePacker Importer
                                    unreal-paper2d      Format for UnrealEngine / Paper2d
                                    vplay               JSON file for V-Play engine
                                    wave-engine-1       WaveEngine Sprite Sheet
                                    x2d                 Export to x2d engine format.
                                    xml                 Generic XML format
                                    plain               Exporter to demonstrate how to crate your own exporters

  --data <filename>             Name of the data file to write

  --class-file <filename>       Name of the class output file (spritekit-swift)

  --classfile-file <filename>   Name of the classfile output file (monogame)

  --header-file <filename>      Name of the header output file (cocos2d-x, spritekit, plain)

  --source-file <filename>      Name of the source output file (cocos2d-x)

  --force-publish               Ignore smart update hash and force re-publishing of the files
  --texturepath <path>          Adds the path to the texture file name stored in the data file.
                                Use this if your sprite sheets are not stored in another folder than your data files.
  --trim-sprite-names           Removes .png, .bmp and .jpg from sprite names
  --prepend-folder-name         Adds the smart folders name to the sprite names
  --replace <regexp>=<string>   Replaces matching parts of the sprite's name with <string>
                                Uses full regular expressions, make sure to escape the expression
  --ignore-files <wildcard>     Ignores all images fitting the given pattern (may be used several times)
                                You can use * and ?, make sure to escape the wildcards when working with bash

  Scaling variants:
  --variant <expr>              Adds a scaled variant of the sheet. Format of the expr
                                <scale>:<name>[:<filter>[:allowfraction][:<width>:<height>]]
                                   <scale>          floating point value, e.g. 0.5
                                   <name>           name of the variant, used to replace {v} in file names, e.g. @2x
                                   <filter>         only sprites which match this filter will be added to the variant
                                   allowfraction    allow floating point values for this scaling if no common
                                                    base factor can be calculated (force identical layout)
                                   <width>:<height> optional maximum size of the texture, if not set the
                                                    maximum texture size will be used (default: 2048x2048)
  --force-identical-layout      Preserves the layout across multiple scaling variants
                                Might require enabling allowfraction on some variants if no common
                                base factor can be derived

  Algorithm settings:
  --algorithm <name>            Choose algorithm
                                    MaxRects        Powerful packing algorithm (extended)
                                    Basic           Simple algorithm for tilemaps and atlases (free)
                                    Polygon         Tight polygon packing
    MaxRects
      --maxrects-heuristics     Heuristic for MaxRects algorithm
                                    Best              Tests all available placements and uses the result with the least used space
                                    ShortSideFit      ShortSideFit
                                    LongSideFit       LongSideFit
                                    AreaFit           AreaFit
                                    BottomLeft        BottomLeft
                                    ContactPoint      ContactPoint

    Polygon
      --align-to-grid <int>     Ensures that the top-left corners of the untrimmed sprites are placed on
                                sprite sheet coordinates divisible by the passed value

    Basic
      --basic-sort-by           Sort order for the sprite list
                                    Best              Tests all sorting variants and uses the result with the least used space
                                    Name              Sorts sprites by name
                                    Width             Sorts sprites by width
                                    Height            Sorts sprite by height
                                    Area              Sorts sprites by their area (width*height)
                                    Circumference     Sorts sprites by their circumference (width+height)
      --basic-order             Sorting direction
                                    Ascending         Sorts values from small to large
                                    Descending        Sorts values from large to small


  Dimensions and layout:
  --width <int>                 Sets fixed width for texture
  --height <int>                Sets fixed height for texture
  --max-width <int>             Sets the maximum width for the texture in auto size mode, default is 2048
  --max-height <int>            Sets the maximum height for the texture in auto size mode, default is 2048
  --max-size <int>              Sets the maximum width and height for the texture in auto size mode, default is 2048
  --size-constraints <value>    Restrict sizes
                                    POT               Power of 2 (2,4,8,16,32,...)
                                    WordAligned       Texture width is multiple of 2 (for 16-bit formats)
                                    AnySize           Any size
  --force-squared               Force squared texture
  --pack-mode <mode>            Optimization mode: Fast, Good, Best
  --multipack                   Create multiple sprite sheets if not all sprites match into a single one
  --common-divisor-x <int>      Resizes sprites - widths will be divisible by this value
  --common-divisor-y <int>      Resizes sprites - heights will be divisible by this value
  --default-pivot-point <x>,<y> Sets default pivot point used for sprites passed on command line

  Padding and rotation:
  --shape-padding <int>         Sets a padding around each shape, value is in pixels
  --border-padding <int>        Sets a padding around each the border, value is in pixels
  --padding <int>               Sets a padding around each shape, and to the border, value is in pixels
  --enable-rotation             Enables rotation of sprites (overriding file format's defaults)
  --disable-rotation            Disables rotation of sprites (overriding file format's defaults)
  --trim-mode <value>           Remove transparent parts of a sprite to shrink atlas size and speed up rendering
                                    None              Keep transparent pixels
                                    Trim              Remove transparent pixels, use original size.
                                    Crop              Remove transparent pixels, use trimmed size, flush position.
                                    CropKeepPos       Remove transparent pixels, use trimmed size, keep position.
                                    Polygon           Approximate sprite contour with polygon path.
  --trim-threshold <int>        Trim alpha values under the threshold value 1..255, default is 1
  --trim-margin <int>           Transparent margin which is left over after trimming
  --tracer-tolerance <int>      Deviation of the polygon approximation from the exact sprite outline, default is 200
  --disable-auto-alias          Disables automated alias creation

  Graphics optimization (extended only):
  --opt <pixelformat>           Optimized output for given pixel formats. Supported formats are:
                                    RGBA8888           32bit, 8bit/channel, 8bit transparency
                                    RGBA4444           16bit, 4bit/channel, 4bit transparency
                                    RGBA5551           16bit, 5bit/channel, 1bit transparancy
                                    RGBA5555           20bit, 5bit/channel, 5bit transparancy
                                    BGRA8888           32bit, 8bit/channel, 8bit transparency
                                    RGB888             24bit, 8bit/channel, no transparency
                                    RGB565             16bit, 5bit red, 6bit green, 5bit blue, no transparancy
                                    PVRTCI_2BPP_RGBA   PVRTC compression, 2bit per pixel
                                    PVRTCI_4BPP_RGBA   PVRTC compression, 4bit per pixel
                                    PVRTCI_2BPP_RGB    PVRTC compression, 2bit per pixel
                                    PVRTCI_4BPP_RGB    PVRTC compression, 4bit per pixel
                                    PVRTCII_2BPP       PVRTC2 compression, 2bit per pixel
                                    PVRTCII_4BPP       PVRTC2 compression, 4bit per pixel
                                    ETC1_RGB           ETC1 compression
                                    ETC1_A             ETC1 Alpha channel only
                                    ETC1_RGB_A         ETC1 RGB + ETC1 Alpha
                                    ETC2_RGB           ETC2 compression
                                    ETC2_RGBA          ETC2 Alpha
                                    DXT1               Compressed with DXT1, 1 bit transparency
                                    DXT5               Compressed with DXT5, transparency
                                    ATF_RGB            ETC1+DXT1+PVRTC4, no transparency
                                    ATF_RGBA           ETC1/ETC1+DXT5+PVRTC4, no transparency
                                    ALPHA              8bit transparency
                                    ALPHA_INTENSITY    8bit intensity, 8bit transparency
  --dither-type <dithertype>    Dithering to improve quality of color reduced images
                                    NearestNeighbour      no dithering
                                    Linear                no dithering
                                    FloydSteinberg        Floyd Steinberg, no alpha
                                    FloydSteinbergAlpha   Floyd Steinberg, with alpha
                                    Atkinson              Atkinson, no alpha
                                    AtkinsonAlpha         Atkinson, alpha
                                    PngQuantLow           PNG-8 only: minimum dithering
                                    PngQuantMedium        PNG-8 only: medium dithering
                                    PngQuantHigh          PNG-8 only: strong dithering
  --background-color <rrggbb>   Set solid background color, default is none, which is transparent
                                The value is a tuple of 3 hexadezimal digit pairs, each pair represents
                                a color channel in order red, green, blue, E.g. ff0000 for red, ffffff for white
  --jpg-quality <value>         Sets the quality for jpg export: -1 for default, 0..100 where 0 is low quality
  --flip-pvr                    Flips PVR files vertically (used for unity framework)
  --pvr-quality <value>         Set quality for PVRTC codecs
                                   verylow - low quality, fast
                                   low
                                   normal
                                   high
                                   best     - best quality, very slow (default)
  --etc1-quality <value>        Set quality for ETC1 compression: low, low-perceptual (default), high, high-perceptual
  --etc2-quality <value>        Set quality for ETC2 compression: low, low-perceptual (default), high, high-perceptual
  --dxt-mode <value>            DXT1/5 compression mode: linear, perceptual
  --jxr-color-mode <value>      Color sampling mode for JXR compression: YUV444, YUV422, YUV420
  --jxr-compression <value>     Compression level for JXR. 0=lossless, 1..100 lossy compression
  --jxr-trim-flexbits <value>   Number of flexbits to trim, 0 (default) .. 15
  --atf-formats <value>         Compression formats to include in ATF_RGB and ATF_RGBA, comma separated.
                                   Default: ETC1,ETC2,DXT,PVRTC
  --compress-atf                Adds JXR compression after ETC/PVRTC/DXT
  --mipmap-min-size <value>     Minimum size of the mipmaps to create, default 32768, ATF only
  --alpha-handling <value>      Defines how color values of transparent pixels are processed:
                                    KeepTransparentPixels    Transparent pixels are copied from sprite to sheet without any change
                                    ClearTransparentPixels   Color values of transparent pixels are set to 0 (transparent black)
                                    ReduceBorderArtifacts    Transparent pixels get color of nearest solid pixel
                                    PremultiplyAlpha         All color values are multiplied with the alpha values
  --dpi <value>                 Set dpi for output image (default is 72)
  --heuristic-mask              Removes equal colored pixels from the border of sprites. Creating a transparency mask
                                on sprites which use one unique color as transparent color.
  --png-opt-level <value>       Optimization level for pngs (0=off, 1=use 8-bit, 2..7=png-opt)
  --webp-quality <value>        Quality level for WebP format (0=low, 100=high, >100=lossless), default is lossless
  --content-protection <key>    Content protection: Encrypt pvr.ccz files.
                                Key: 128 bit, 32 hex digits [A-F0-9]Key: 128 bit, 32 hex digits [A-F0-9]

  --extrude <int>               Extrudes the sprites by given value of pixels to fix flickering problems in tile maps
  --scale <float>               Scales all images before creating the sheet. E.g. use 0.5 for half size
  --scale-mode <mode>           Use mode for scaling:
                                    Smooth            Smooth
                                    Fast              Fast (Nearest Neighbor)
                                    Scale2x           Scale2x (fixed 2x upscaling)
                                    Scale3x           Scale3x (fixed 3x upscaling)
                                    Scale4x           Scale4x (fixed 4x upscaling)
                                    Eagle             Eagle2x (fixed 2x upscaling)
                                    Hq2x              Hq2x (fixed 2x upscaling)

  Normal maps:
  --pack-normalmaps             Packs normal maps on separate sheet, with same layout as sprites
  --normalmap-filter <string>   Sprites with file paths containing this string are treated as normal maps
  --normalmap-suffix <string>   Suffix which is added to a sprite name to find the corresponding normal map
  --normalmap-sheet <filename>  File to which the normal map sprite sheet is written

  Custom Exporters
  --custom-exporters-directory <dirname> Search for additional custom exporters in this folder


  Additional settings for "CSS (responsive, retina)" (--format css):
  --css-media-query-2x <string>          Media Query used for the -2x variant (default: "(-webkit-min-device-pixel-ratio: 2), (min-resolution: 192dpi)")
  --css-sprite-prefix <string>           Prefix for the sprite's name. Works best with prefixes like "icon-". (default: "")

  Additional settings for "EaselJS / CreateJS" (--format easeljs):
  --easeljs-framerate <string>           Framerate (default: "20")

  Additional settings for "~~~ Custom ~~~" (--format plain):
  --plain-string-property <string>       Some string that is passed to the exporter, you can access it using {{exporterProperties.string_property}} (default: "hello world")
  --plain-bool-property [true|false]     Some bool flag that is passed to the exporter, you can access it using {{exporterProperties.bool_property}} (default: false)

  Debugging:
  --shape-debug                 Creates boxes around shapes for debugging
  --verbose                     Be verbose
  --quiet                       No output except for errors

  License management:
  --license-info                Prints information about the currently installed license
  --activate-license <key>      Activate a license key


Examples:

  TexturePacker assets
        creates out.plist (cocos2d) and out.png from all png files in the 'assets' directory
        trimming all files and creating a texture with max. 2048x2048px

  TexturePacker --data main-hd.plist --format cocos2d --sheet main-hd.png assets
        same as above, but with output files main-hd.plist, main-hd.png

  TexturePacker --scale 0.5 --max-size 1024 --data main.plist --format cocos2d --sheet main.png assets
        creates main.plist and main.png from all files in assets
        scaling all images to 50%, trimming all files and creating
        a texture with max. 1024x1024px

  TexturePacker --variant 1:-hd --variant 0.5: --data main{v}.plist --sheet main{v}.png --format cocos2d assets
        combines the two TexturePacker calls shown above: it creates variants with scaling
        factors 1.0 and 0.5, and stores them in main.plist/png and main-hd.plist/png
        ({v} is replaced by variant name)

  TexturePacker --variant 1:-hd:@2x --variant 1: --data main{v}.plist --sheet main{v}.png --format cocos2d assets
        similar to the example before, but TexturePacker expects that you already provide sprites
        named like 'a.png', 'b.png', 'a@2x.png', 'b@2x.png'. It selects the '@2x' sprites and put them
        on the 'main-hd' sprite sheet, and the other ones on the 'main' sprite sheet.

  TexturePacker --data main.atlasc --format spritekit assets
        the SpriteKit exporter uses variant filters by default, and puts sprites with the following suffixes
        to separate sprite sheets:
        '~iphone', '~ipad', '@2x~iphone', '@2x~ipad', '@2x', '@3x~iphone', '@3x', all remaining sprites
*/


/*

打包图集
子目录的配置会继承/覆盖上一目录的配置
配置需要填写android和ios两部分,缺失的配置会继承父目录,根目录继承defalut config
e.g.:

{
    "Assets/Sprites": {
        "algorithm" : "Polygon",
        "forcesquared" : "true",
        "maxsize" : 512,
        "textureformat" : "tga",
        "pixelformat" : "ETC1_RGB_A",
        "dithertype" : "PngQuantHigh"
    }
}

 */
#if UNITY_EDITOR
namespace Niuwa
{
    public class BuildAltas
    {       
        public enum texture_format 
        {
            png             ,
            png8            ,
            pvr3            ,
            pvr3gz          ,
            pvr3ccz         ,
            jpg             ,
            bmp             ,
            tga             ,
            tiff            ,
            pkm             ,
            webp            ,
            atf             ,
        }
        
        public enum optimization
        {
            RGBA8888        ,
            RGBA4444        ,
            RGBA5551        ,
            RGBA5555        ,
            BGRA8888        ,
            RGB888          ,
            RGB565          ,
            PVRTCI_2BPP_RGBA,
            PVRTCI_4BPP_RGBA,
            PVRTCI_2BPP_RGB ,
            PVRTCI_4BPP_RGB ,
            PVRTCII_2BPP    ,
            PVRTCII_4BPP    ,
            ETC1_RGB        ,
            ETC1_A          ,
            ETC1_RGB_A      ,
            ETC2_RGB        ,
            ETC2_RGBA       ,
            DXT1            ,
            DXT5            ,
            ATF_RGB         ,
            ATF_RGBA        ,
            ALPHA           ,
            ALPHA_INTENSITY ,
        }
        
        public enum dither_type
        {
            NearestNeighbour    ,
            Linear              ,
            FloydSteinberg      ,
            FloydSteinbergAlpha ,
            Atkinson            ,
            AtkinsonAlpha       ,
            PngQuantLow         ,
            PngQuantMedium      ,
            PngQuantHigh        ,
        }

        public struct FolderPackInfo
        {
            public int maxsize;
            public string extension;
            public string textureformat;
            public string pixelformat;
            public string dithertype;
            public string forcesquared;
            public string algorithm;
            public string trimmode;

            public FolderPackInfo(int maxsize, texture_format fmt, optimization opt, dither_type dither, string forcesquared)
            {
                this.maxsize = maxsize;
                this.extension = "png";
                this.algorithm = "Polygon";
                this.trimmode = "None";
                this.textureformat = fmt.ToString();
                this.pixelformat = opt.ToString();
                this.dithertype = dither.ToString();
                this.forcesquared = forcesquared;
            }
        }

        public static FolderPackInfo defaultInfo = new FolderPackInfo(2048, texture_format.png8, optimization.RGBA8888, dither_type.PngQuantHigh, "false");

        private static Dictionary<string, FolderPackInfo> mFolderPackInfo;

        private static string TPInstallDir = Application.dataPath + "/../Tools/TexturePacker/bin/TexturePacker.exe";

        private const string OutPutDirRoot = "Assets/GameMain/Altas/";

        [MenuItem ("Assets/打包所有图集", false, 0)]
        static void StartBuildAltasAndroid ()
        {
            TextAsset t = AssetDatabase.LoadMainAssetAtPath("Assets/Sprites/packInfo.json") as TextAsset;
            if (t)
            {
                mFolderPackInfo = LitJson.JsonMapper.ToObject<Dictionary<string, FolderPackInfo>> (t.text);
            }
            
            Niuwa.BuildAltas.BuildFolder ("Assets/Sprites", Niuwa.BuildAltas.defaultInfo);
        }

        [MenuItem ("Assets/打包选中目录图集", false, 0)]
        static void StartBuildCurrentAltasAndroid ()
        {
            TextAsset t = AssetDatabase.LoadMainAssetAtPath("Assets/Sprites/packInfo.json") as TextAsset;
            if (t)
            {
                mFolderPackInfo = LitJson.JsonMapper.ToObject<Dictionary<string, FolderPackInfo>> (t.text);
            }

            string[] strs = Selection.assetGUIDs;
            
            foreach (var item in strs)
            {
                BuildFolder (AssetDatabase.GUIDToAssetPath (item), defaultInfo);
            }
        }

        public static void BuildOneTP (string folderPath, FolderPackInfo info)
        {
            if (Directory.Exists (folderPath))
            {
                if(!folderPath.Contains(" "))
                {
                    StringBuilder sb = new StringBuilder ("");
                    string[] fileName = Directory.GetFiles (folderPath).Where(t => (Path.GetExtension (t) == ".png" || Path.GetExtension(t) == ".jpg")).ToArray();
                    string[] tpsfile = Directory.GetFiles (folderPath).Where(t => (Path.GetExtension (t) == ".tps")).ToArray();
                   
                    if(tpsfile.Length > 0)
                    {
                        string sheetName = OutPutDirRoot + GetSheetName(folderPath);
                        
                        foreach (var tps in tpsfile)
                        {
                            UnityEngine.Debug.Log($"使用 {tps} 配置打包图集,此文件夹对应的Json配置将失效");

                            string command = $" --sheet {sheetName}.{info.extension} --data {sheetName}.tpsheet ";
                            AutoBuild.processCommand (TPInstallDir, command + tps);
                        }
                    }
                    else
                    {
                        if(fileName.Length > 0)
                        {
                            GetImageName (fileName, ref sb);

                            string sheetName = OutPutDirRoot + GetSheetName(folderPath);

                            string command = $" --sheet {sheetName}.{info.extension} --data {sheetName}.tpsheet --extrude 0 --format unity-texture2d --trim-mode {info.trimmode} --default-pivot-point 0.5,0.5 --pack-mode Best --algorithm {info.algorithm} --max-size {info.maxsize} --size-constraints POT --opt {info.pixelformat} --texture-format {info.textureformat} --dither-type {info.dithertype} --scale 1 --disable-rotation ";
                            if(info.forcesquared == "true")
                            {
                                command = command + " --force-squared ";
                            }

                            AutoBuild.processCommand (TPInstallDir, command + sb.ToString ());
                        }
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"{folderPath} 不能有空格");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"{folderPath} is not directory path");
            }
        }

        public static string GetSheetName(string folderPath)
        {
            string name = folderPath.Replace("Assets/", "");
            name = name.Replace("\\", "_");
            name = name.Replace("/", "_");
            name = name.Replace(".", "_");
            return name;
        }

        public static void BuildFolder(string folderPath, FolderPackInfo parentInfo)
        {
            if(mFolderPackInfo == null)
            {
                TextAsset t = AssetDatabase.LoadMainAssetAtPath("Assets/Sprites/packInfo.json") as TextAsset;
                if (t)
                {
                    mFolderPackInfo = LitJson.JsonMapper.ToObject<Dictionary<string, FolderPackInfo>> (t.text);
                }
            }

            folderPath = GameFramework.Utility.Path.GetRegularPath(folderPath);

            if(folderPath == "Assets")
            {
                BuildFolder("Assets/Sprites", parentInfo);
                return;
            }

            if(folderPath.Contains("Assets/GameMain"))
            {
                throw new System.Exception($"GameMain里面是热更资源,需要打包的散图应该放在这个目录外面");
            }

            FolderPackInfo info = parentInfo;
            FolderPackInfo platformInfo;

            if(mFolderPackInfo != null && mFolderPackInfo.ContainsKey(folderPath))
            {
                mFolderPackInfo.TryGetValue(folderPath, out platformInfo);

                info = platformInfo;

                info.algorithm = info.algorithm ?? parentInfo.algorithm;
                info.trimmode = info.trimmode ?? parentInfo.trimmode;
                info.forcesquared = info.forcesquared ?? parentInfo.forcesquared;
                info.extension = info.extension ?? parentInfo.extension;
                info.dithertype = info.dithertype ?? parentInfo.dithertype;
                info.textureformat = info.textureformat ?? parentInfo.textureformat;
                info.maxsize = info.maxsize == 0 ? parentInfo.maxsize : info.maxsize;
                info.pixelformat = info.pixelformat ?? parentInfo.pixelformat;
            }
            
            if (!Directory.Exists (folderPath))
            {
                throw new System.Exception($"{folderPath} is not directory path");
            }
            
            BuildOneTP(folderPath, info);

            foreach (string sub in Directory.GetDirectories (folderPath))
            {
                BuildFolder (sub, info);
            }
        }

        private static StringBuilder GetImageName (string[] fileName, ref StringBuilder sb)
        {
            for (int j = 0; j < fileName.Length; j++)
            {
                sb.Append (fileName[j]);
                sb.Append ("  ");
            }
            return sb;
        }

        
    }
}
#endif