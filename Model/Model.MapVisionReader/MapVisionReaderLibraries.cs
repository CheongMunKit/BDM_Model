using BDMVision.Model.Enum;
using BDMVision.Model.MapVision;
using Euresys.Open_eVision_2_0;
using System;
using System.Collections.Generic;

namespace BDMVision.Model.MapVisionReader
{
    public class MapVisionReaderLibraries
    {
        public static MapDataFromVision ReadImage(
            MapVisionParameters recipe, 
            EImageBW8 eImage)
        {
            if (recipe == null) throw new ArgumentNullException("recipe");
            if (eImage == null) throw new ArgumentNullException("eImage");

            int rowCount = recipe.RowCount;
            int columnCount = recipe.ColumnCount;
            float startingPointX = recipe.StartingPointX;
            float startingPointY = recipe.StartingPointY;
            float mapWidth = recipe.DieWidth;
            float mapHeight = recipe.DieHeight;
            List<List<BDMMapFromVision>> MapFromVisionListofList = new List<List<BDMMapFromVision>>();

            for (int i = 0; i < rowCount; i++)
            {
                List<BDMMapFromVision> MapFromVisionList = new List<BDMMapFromVision>() ;
                for (int j = 0; j < columnCount; j++)
                { 
                    EROIBW8 currentROI = new EROIBW8();
                    currentROI.Attach(eImage);  
                    float ROIpositionX = startingPointX + j * mapWidth;
                    float ROIpositionY = startingPointY + i * mapHeight;  
                    currentROI.OrgX = (int)Math.Round(ROIpositionX);
                    currentROI.OrgY = (int)Math.Round(ROIpositionY);
                    currentROI.Width = (int)Math.Round(mapWidth);
                    currentROI.Height = (int)Math.Round(mapHeight);
                    VisionMapCategory visionMapCategory;
                    
                    // if roi out of image border
                    if(currentROI.OrgX + currentROI.Width  >= eImage.Width  ||
                       currentROI.OrgY + currentROI.Height >= eImage.Height ||
                       currentROI.OrgX < 0                                  || 
                       currentROI.OrgY < 0)
                    {
                        visionMapCategory = VisionMapCategory.OutOfBound;
                    }

                    else { visionMapCategory = CalculateMapCategory(currentROI, recipe); }
                        
                    BDMMapFromVision currentMap = new BDMMapFromVision()
                    {
                        mapCategory = visionMapCategory,
                        PointX = currentROI.OrgX,
                        PointY = currentROI.OrgY,
                        Width = mapWidth,
                        Height = mapHeight,
                    };
                    MapFromVisionList.Add(currentMap);
                }
                MapFromVisionListofList.Add(MapFromVisionList);
            }

            return new MapDataFromVision()
            {
                MapsFromVision = MapFromVisionListofList,
            };
        }      
        
        /// <summary>
        /// Average Pixel Value
        /// 0 if fully black
        /// 255 if fully white
        /// </summary>
        /// <param name="roi"></param>
        /// <param name="recipe"></param>
        /// <returns></returns>
        private static VisionMapCategory CalculateMapCategory(
            EROIBW8 roi, 
            MapVisionParameters recipe)
        {
            float DiePresentThreshold = recipe.DiePresentThreshold;
            float averagePixelValue;
            EasyImage.PixelAverage(roi, out averagePixelValue);
            VisionMapCategory visionMapCategory = VisionMapCategory.Undefined;
            //if (averagePixelValue > TakenDieThreshold && averagePixelValue <= 256) visionMapCategory = VisionMapCategory.Taken;
            //else if (averagePixelValue > GoodDieThreshold && averagePixelValue <= TakenDieThreshold) visionMapCategory = VisionMapCategory.GoodDie;
            //else if (averagePixelValue > BadDieThreshold && averagePixelValue <= GoodDieThreshold) visionMapCategory = VisionMapCategory.BadDie; 

            if (averagePixelValue > DiePresentThreshold) visionMapCategory = VisionMapCategory.DieTaken;
            else if (averagePixelValue <= DiePresentThreshold) visionMapCategory = VisionMapCategory.DieRemain;
            else throw new Exception("Invalid vision Map Category");  
            return visionMapCategory;
        }            
    }
}
