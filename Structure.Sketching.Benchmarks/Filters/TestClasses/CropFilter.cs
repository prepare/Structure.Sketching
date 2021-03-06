﻿using Structure.Sketching.Colors;
using Structure.Sketching.Filters.Interfaces;
using Structure.Sketching.Numerics;
using System.Threading.Tasks;

namespace Structure.Sketching.Benchmarks.Filters.TestClasses
{
    /// <summary>
    /// Crops the image
    /// </summary>
    /// <seealso cref="Structure.Sketching.Filters.Interfaces.IFilter"/>
    public class CropFilter : IFilter
    {
        /// <summary>
        /// Applies the filter to the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="targetLocation">The target location.</param>
        /// <returns>The image</returns>
        public unsafe Image Apply(Image image, Rectangle targetLocation = default(Rectangle))
        {
            targetLocation = targetLocation == default(Rectangle) ? new Rectangle(0, 0, image.Width, image.Height) : targetLocation.Clamp(image);
            var Result = new Color[targetLocation.Width * targetLocation.Height];
            Parallel.For(targetLocation.Bottom, targetLocation.Top, y =>
            {
                fixed (Color* SourcePointer = image.Pixels)
                {
                    fixed (Color* TargetPointer = Result)
                    {
                        Color* TargetPointer2 = TargetPointer + ((y - targetLocation.Bottom) * targetLocation.Width);
                        Color* SourcePointer2 = SourcePointer + ((y * image.Width) + targetLocation.Left);
                        for (int x = targetLocation.Left; x < targetLocation.Right; ++x)
                        {
                            *(TargetPointer2++) = *(SourcePointer2++);
                        }
                    }
                }
            });
            return image.ReCreate(targetLocation.Width, targetLocation.Height, Result);
        }
    }
}