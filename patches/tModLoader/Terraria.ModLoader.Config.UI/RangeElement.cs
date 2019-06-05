﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria.GameInput;
using Terraria.UI;

namespace Terraria.ModLoader.Config.UI
{
	public abstract class PrimitiveRangeElement<T> : RangeElement where T : IComparable<T>
	{
		public int index;
		public T min;
		public T max;
		public T increment;
		public new IList<T> array;

		public PrimitiveRangeElement(PropertyFieldWrapper memberInfo, object item, IList<T> array = null, int index = -1) : base(memberInfo, item, (IList)array) {
			this.array = array;
			this.index = index;
			this._TextDisplayFunction = () => memberInfo.Name + ": " + GetValue();

			if (array != null) {
				_TextDisplayFunction = () => index + 1 + ": " + array[index];
			}

			if (labelAttribute != null) // Problem with Lists using ModConfig Label.
			{
				this._TextDisplayFunction = () => labelAttribute.Label + ": " + GetValue();
			}
			if (rangeAttribute != null && rangeAttribute.min is T && rangeAttribute.max is T) {
				min = (T)rangeAttribute.min;
				max = (T)rangeAttribute.max;
			}
			if (incrementAttribute != null && incrementAttribute.increment is T) {
				this.increment = (T)incrementAttribute.increment;
			}
		}

		protected virtual void SetValue(T value) {
			if (array != null) {
				array[index] = value;
				Interface.modConfig.SetPendingChanges();
				return;
			}
			if (!memberInfo.CanWrite) return;
			memberInfo.SetValue(item, Utils.Clamp(value, min, max));
			Interface.modConfig.SetPendingChanges();
		}

		protected virtual T GetValue() {
			if (array != null)
				return array[index];
			return (T)memberInfo.GetValue(item);
		}
	}

	public abstract class RangeElement : ConfigElement
	{
		protected Color sliderColor = Color.White;
		protected Utils.ColorLerpMethod colorMethod;
		internal bool drawTicks;
		public abstract int NumberTicks { get; }
		public abstract float TickIncrement { get; }

		protected abstract float Proportion {
			get;
			set;
		}

		public RangeElement(PropertyFieldWrapper memberInfo, object item, IList array) : base(memberInfo, item, array)
		{
			drawTicks = Attribute.IsDefined(memberInfo.MemberInfo, typeof(DrawTicksAttribute));
			sliderColor = ConfigManager.GetCustomAttribute<SliderColorAttribute>(memberInfo, item, array)?.color ?? Color.White;
			colorMethod = new Utils.ColorLerpMethod((percent) => Color.Lerp(Color.Black, sliderColor, percent));
		}

		public float DrawValueBar(SpriteBatch sb, float scale, float perc, int lockState = 0, Utils.ColorLerpMethod colorMethod = null)
		{
			perc = Utils.Clamp(perc, -.05f, 1.05f);
			if (colorMethod == null)
			{
				colorMethod = new Utils.ColorLerpMethod(Utils.ColorLerp_BlackToWhite);
			}
			Texture2D colorBarTexture = Main.colorBarTexture;
			Vector2 vector = new Vector2((float)colorBarTexture.Width, (float)colorBarTexture.Height) * scale;
			IngameOptions.valuePosition.X = IngameOptions.valuePosition.X - (float)((int)vector.X);
			Rectangle rectangle = new Rectangle((int)IngameOptions.valuePosition.X, (int)IngameOptions.valuePosition.Y - (int)vector.Y / 2, (int)vector.X, (int)vector.Y);
			Rectangle destinationRectangle = rectangle;
			int num = 167;
			float num2 = (float)rectangle.X + 5f * scale;
			float num3 = (float)rectangle.Y + 4f * scale;
			if (drawTicks)
			{
				int numTicks = NumberTicks;
				if (numTicks > 1)
				{
					for (int tick = 0; tick < numTicks; tick++)
					{
						float percent = tick * TickIncrement;
						if (percent <= 1f)
							sb.Draw(Main.magicPixel, new Rectangle((int)(num2 + num * percent * scale), rectangle.Y - 2, 2, rectangle.Height + 4), Color.White);
					}
				}
			}
			sb.Draw(colorBarTexture, rectangle, Color.White);
			for (float num4 = 0f; num4 < (float)num; num4 += 1f)
			{
				float percent = num4 / (float)num;
				sb.Draw(Main.colorBlipTexture, new Vector2(num2 + num4 * scale, num3), null, colorMethod(percent), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			}
			rectangle.Inflate((int)(-5f * scale), 2);
			//rectangle.X = (int)num2;
			//rectangle.Y = (int)num3;
			bool flag = rectangle.Contains(new Point(Main.mouseX, Main.mouseY));
			if (lockState == 2)
			{
				flag = false;
			}
			if (flag || lockState == 1)
			{
				sb.Draw(Main.colorHighlightTexture, destinationRectangle, Main.OurFavoriteColor);
			}
			sb.Draw(Main.colorSliderTexture, new Vector2(num2 + 167f * scale * perc, num3 + 4f * scale), null, Color.White, 0f, new Vector2(0.5f * (float)Main.colorSliderTexture.Width, 0.5f * (float)Main.colorSliderTexture.Height), scale, SpriteEffects.None, 0f);
			if (Main.mouseX >= rectangle.X && Main.mouseX <= rectangle.X + rectangle.Width)
			{
				IngameOptions.inBar = flag;
				return (float)(Main.mouseX - rectangle.X) / (float)rectangle.Width;
			}
			IngameOptions.inBar = false;
			if (rectangle.X >= Main.mouseX)
			{
				return 0f;
			}
			return 1f;
		}

		private static RangeElement rightLock;
		private static RangeElement rightHover;
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			float num = 6f;
			int num2 = 0;
			//IngameOptions.rightHover = -1;
			rightHover = null;
			if (!Main.mouseLeft)
			{
				//IngameOptions.rightLock = -1;
				rightLock = null;
			}
			//if (IngameOptions.rightLock == this._sliderIDInPage)
			if (rightLock == this)
			{
				num2 = 1;
			}
			//else if (IngameOptions.rightLock != -1)
			else if (rightLock != null)
			{
				num2 = 2;
			}
			CalculatedStyle dimensions = base.GetDimensions();
			float num3 = dimensions.Width + 1f;
			Vector2 vector = new Vector2(dimensions.X, dimensions.Y);
			bool flag2 = base.IsMouseHovering;
			if (num2 == 1)
			{
				flag2 = true;
			}
			if (num2 == 2)
			{
				flag2 = false;
			}
			Vector2 vector2 = vector;
			vector2.X += 8f;
			vector2.Y += 2f + num;
			vector2.X -= 17f;
			Main.colorBarTexture.Frame(1, 1, 0, 0);
			vector2 = new Vector2(dimensions.X + dimensions.Width - 10f, dimensions.Y + 10f + num);
			IngameOptions.valuePosition = vector2;
			float obj = DrawValueBar(spriteBatch, 1f, Proportion, num2, colorMethod);
			//if (IngameOptions.inBar || IngameOptions.rightLock == this._sliderIDInPage)
			if (IngameOptions.inBar || rightLock == this)
			{
				rightHover = this;
				//IngameOptions.rightHover = this._sliderIDInPage;
				if (PlayerInput.Triggers.Current.MouseLeft && rightLock == this)
				//if (PlayerInput.Triggers.Current.MouseLeft && IngameOptions.rightLock == this._sliderIDInPage)
				{
					Proportion = obj;
				}
			}
			if (rightHover != null && rightLock == null)
			//if (IngameOptions.rightHover != -1 && IngameOptions.rightLock == -1)
			{
				//IngameOptions.rightLock = IngameOptions.rightHover;
				rightLock = rightHover;
			}
		}
	}
}
