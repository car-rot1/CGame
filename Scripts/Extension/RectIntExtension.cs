﻿using System.Linq;
using UnityEngine;

namespace CGame
{
    public static class RectIntExtension
    {
        public static void Deconstruct(this RectInt self, out int x, out int y, out int width, out int height)
        {
            x = self.x;
            y = self.y;
            width = self.width;
            height = self.height;
        }
        
        public static void Deconstruct(this RectInt self, out Vector2Int position, out Vector2Int size)
        {
            position = self.position;
            size = self.size;
        }
        
        public static RectInt[] HorizontalSplit(this RectInt self, params int[] widths)
        {
            if (widths.Length <= 0)
                return new[] { self };
            
            var remainWidthSum = Mathf.Max(0, self.width - widths.Where(w => w > 0).Sum());
            var pctWidthSum = widths.Where(w => w < 0).Select(w => -w).Sum();
            
            for (var i = 0; i < widths.Length; i++)
            {
                if (widths[i] < 0)
                {
                    widths[i] = remainWidthSum * (-widths[i] / pctWidthSum);
                }
            }

            var result = new RectInt[widths.Length];
            var x = self.xMin;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = self;
                result[i].xMin = x;
                result[i].xMax = x + widths[i];
                x += widths[i];
            }

            return result;
        }
        
        public static RectInt[] HorizontalEquallySplitForWidth(this RectInt self, int width)
        {
            var num = self.width / width;
            if (num <= 1)
                return new[] { self };
            
            var result = new RectInt[num];
            var x = self.xMin;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = self;
                result[i].xMin = x;
                result[i].xMax = x + width;
                x += width;
            }

            return result;
        }
        
        public static RectInt[] HorizontalEquallySplitForNum(this RectInt self, int num)
        {
            if (num <= 1)
                return new[] { self };
            
            var width = self.width / num;
            var result = new RectInt[num];
            var x = self.xMin;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = self;
                result[i].xMin = x;
                result[i].xMax = x + width;
                x += width;
            }

            return result;
        }
        
        public static RectInt[] VerticalSplit(this RectInt self, params int[] heights)
        {
            if (heights.Length <= 0)
                return new[] { self };
            
            var remainHeightSum = Mathf.Max(0, self.height - heights.Where(h => h > 0).Sum());
            var pctHeightSum = heights.Where(h => h < 0).Select(w => -w).Sum();
            
            for (var i = 0; i < heights.Length; i++)
            {
                if (heights[i] < 0)
                {
                    heights[i] = remainHeightSum * (-heights[i] / pctHeightSum);
                }
            }
            
            var result = new RectInt[heights.Length];
            var y = self.yMin;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = self;
                result[i].yMin = y;
                result[i].yMax = y + heights[i];
                y += heights[i];
            }

            return result;
        }

        public static RectInt[] VerticalEquallySplitForHeight(this RectInt self, int height)
        {
            var num = self.height / height;
            if (num <= 1)
                return new[] { self };
            
            var result = new RectInt[num];
            var y = self.yMin;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = self;
                result[i].yMin = y;
                result[i].yMax = y + height;
                y += height;
            }

            return result;
        }
        
        public static RectInt[] VerticalEquallySplitForNum(this RectInt self, int num)
        {
            if (num <= 1)
                return new[] { self };
            
            var height = self.height / num;
            var result = new RectInt[num];
            var y = self.yMin;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = self;
                result[i].yMin = y;
                result[i].yMax = y + height;
                y += height;
            }

            return result;
        }
        
        public static RectInt[] QuadSplit(this RectInt self, int row, int column)
        {
            row = Mathf.Max(row, 1);
            column = Mathf.Max(column, 1);

            if (row == 1 && column == 1)
                return new[] { self };
            
            var result = new RectInt[row * column];
            
            var width = self.width / column;
            var height = self.height / row;
            var currentPosition = self.position;

            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    result[i * column + j] = new RectInt(currentPosition.x + j * width, currentPosition.y + i * height, width, height);
                }
            }

            return result;
        }
        
#region Padding
        
        public static RectInt Padding(this RectInt self, int left, int right, int top, int bottom)
        {
            self.xMin += left;
            self.xMax -= right;
            self.yMin += top;
            self.yMax -= bottom;
            
            return self;
        }
        
        public static RectInt Padding(this RectInt self, int horizontal, int vertical)
        {
            return Padding(self, horizontal, horizontal, vertical, vertical);
        }
        
        public static RectInt Padding(this RectInt self, int padding)
        {
            return Padding(self, padding, padding, padding, padding);
        }
        
#endregion

#region Margin

        public static RectInt Margin(this RectInt self, int left, int right, int top, int bottom)
        {
            self.xMin -= left;
            self.xMax += right;
            self.yMin -= top;
            self.yMax += bottom;
            
            return self;
        }

        public static RectInt Margin(this RectInt self, int horizontal, int vertical)
        {
            return Margin(self, horizontal, horizontal, vertical, vertical);
        }
        
        public static RectInt Margin(this RectInt self, int margin)
        {
            return Margin(self, margin, margin, margin, margin);
        }
        
#endregion

        public static bool ContainsIncludeBorder(this RectInt self, Vector2Int position)
        {
            return position.x >= self.xMin && position.y >= self.yMin && position.x <= self.xMax && position.y <= self.yMax;
        }
    }
}