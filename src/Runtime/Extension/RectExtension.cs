using System.Linq;
using UnityEngine;

namespace CGame
{
    public static class RectExtension
    {
        public static void Deconstruct(this Rect self, out float x, out float y, out float width, out float height)
        {
            x = self.x;
            y = self.y;
            width = self.width;
            height = self.height;
        }
        
        public static void Deconstruct(this Rect self, out Vector2 position, out Vector2 size)
        {
            position = self.position;
            size = self.size;
        }
        
        public static Rect[] HorizontalSplit(this Rect self, params float[] widths)
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

            var result = new Rect[widths.Length];
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
        
        public static Rect[] HorizontalEquallySplitForWidth(this Rect self, float width)
        {
            var num = Mathf.FloorToInt(self.width / width);
            if (num <= 1)
                return new[] { self };
            
            var result = new Rect[num];
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
        
        public static Rect[] HorizontalEquallySplitForNum(this Rect self, int num)
        {
            if (num <= 1)
                return new[] { self };
            
            var width = self.width / num;
            var result = new Rect[num];
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
        
        public static Rect[] VerticalSplit(this Rect self, params float[] heights)
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
            
            var result = new Rect[heights.Length];
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

        public static Rect[] VerticalEquallySplitForHeight(this Rect self, float height)
        {
            var num = Mathf.FloorToInt(self.height / height);
            if (num <= 1)
                return new[] { self };
            
            var result = new Rect[num];
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
        
        public static Rect[] VerticalEquallySplitForNum(this Rect self, int num)
        {
            if (num <= 1)
                return new[] { self };
            
            var height = self.height / num;
            var result = new Rect[num];
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
        
        public static Rect[,] QuadSplitForSize(this Rect self, float width, float height)
        {
            var row = Mathf.FloorToInt(self.height / height);
            var column = Mathf.FloorToInt(self.width / width);
            if (row <= 1 && column <= 1)
                return new[,] { { self, self } };
            
            var result = new Rect[row, column];
            
            var currentPosition = self.position;

            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    result[i, + j] = new Rect(currentPosition.x + j * width, currentPosition.y + i * height, width, height);
                }
            }

            return result;
        }
        
        public static Rect[,] QuadSplitForNum(this Rect self, int row, int column)
        {
            row = Mathf.Max(row, 1);
            column = Mathf.Max(column, 1);

            if (row == 1 && column == 1)
                return new[,] { { self, self } };
            
            var result = new Rect[row, column];
            
            var width = self.width / column;
            var height = self.height / row;
            var currentPosition = self.position;

            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < column; j++)
                {
                    result[i, j] = new Rect(currentPosition.x + j * width, currentPosition.y + i * height, width, height);
                }
            }

            return result;
        }
        
        
        
#region Padding
        
        public static Rect Padding(this Rect self, float left, float right, float top, float bottom)
        {
            self.xMin += left;
            self.xMax -= right;
            self.yMin += top;
            self.yMax -= bottom;
            
            return self;
        }
        
        public static Rect Padding(this Rect self, float horizontal, float vertical)
        {
            return Padding(self, horizontal, horizontal, vertical, vertical);
        }
        
        public static Rect Padding(this Rect self, float padding)
        {
            return Padding(self, padding, padding, padding, padding);
        }
        
#endregion

#region Margin

        public static Rect Margin(this Rect self, float left, float right, float top, float bottom)
        {
            self.xMin -= left;
            self.xMax += right;
            self.yMin -= top;
            self.yMax += bottom;
            
            return self;
        }

        public static Rect Margin(this Rect self, float horizontal, float vertical)
        {
            return Margin(self, horizontal, horizontal, vertical, vertical);
        }
        
        public static Rect Margin(this Rect self, float margin)
        {
            return Margin(self, margin, margin, margin, margin);
        }
        
#endregion

        public static bool ContainsIncludeBorder(this Rect self, Vector2 position)
        {
            return position.x >= self.xMin && position.y >= self.yMin && position.x <= self.xMax && position.y <= self.yMax;
        }
    }
}