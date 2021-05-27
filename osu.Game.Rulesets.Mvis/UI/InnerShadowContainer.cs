﻿using System;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Mvis.UI
{
    public abstract class InnerShadowContainer : Container
    {
        public new readonly BindableFloat CornerRadius = new BindableFloat();
        public new readonly BindableFloat Depth = new BindableFloat();

        protected override Container<Drawable> Content => content;

        private readonly Container<Drawable> content;
        private readonly BufferedContainer blur;
        private readonly Container outherShadow;
        private readonly Box mainShadow;

        protected InnerShadowContainer()
        {
            Masking = true;
            InternalChildren = new Drawable[]
            {
                content = CreateContent(),
                blur = new BufferedContainer
                {
                    BypassAutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        outherShadow = new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Masking = true,
                            BorderColour = Color4.Black,
                            Child = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0,
                                AlwaysPresent = true
                            }
                        }
                    }
                },
                mainShadow = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black
                }
            };
        }

        protected override void LoadAsyncComplete()
        {
            base.LoadAsyncComplete();
            CornerRadius.BindValueChanged(_ => updateValues());
            Depth.BindValueChanged(_ => updateValues(), true);
        }

        protected abstract Container<Drawable> CreateContent();

        private void updateValues()
        {
            base.CornerRadius = CornerRadius.Value;
            outherShadow.CornerRadius = CornerRadius.Value + Depth.Value;

            outherShadow.BorderThickness = Depth.Value;
            blur.BlurSigma = new Vector2(Depth.Value);
            mainShadow.Alpha = Math.Clamp(Depth.Value / 100, 0, 0.8f);
        }

        protected override void Update()
        {
            base.Update();

            outherShadow.Size = DrawSize + new Vector2(outherShadow.BorderThickness);
            blur.Size = outherShadow.Size + new Vector2(Blur.KernelSize(Depth.Value) * 2);
        }
    }
}
