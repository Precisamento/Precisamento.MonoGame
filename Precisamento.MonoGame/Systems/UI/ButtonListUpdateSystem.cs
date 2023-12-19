using DefaultEcs;
using DefaultEcs.System;
using Microsoft.Xna.Framework;
using Precisamento.MonoGame.Collisions;
using Precisamento.MonoGame.Graphics;
using Precisamento.MonoGame.Input;
using Precisamento.MonoGame.MathHelpers;
using Precisamento.MonoGame.Scenes;
using Precisamento.MonoGame.Timers;
using Precisamento.MonoGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.MonoGame.Systems.UI
{
    public class ButtonListUpdateSystem : AComponentSystem<float, ButtonListComponent>
    {
        private InputManager _input;
        private IActionManager _actions;

        public ButtonListUpdateSystem(World world, Game game)
            : base(world)
        {
            _input = game.Services.GetService<InputManager>();
            _actions = game.Services.GetService<IActionManager>();
        }

        protected override void Update(float state, ref ButtonListComponent list)
        {
            ProcessActionInput(state, ref list);
            ProcessMouseInput(state, ref list);
        }

        private void ProcessMouseInput(float deltaTime, ref ButtonListComponent list)
        {
            var mousePosition = _input.MousePosition.ToVector2();
            if (SceneManager.CurrentScene?.Camera != null)
            {
                mousePosition = SceneManager.CurrentScene.Camera.ScreenToWorld(mousePosition);
            }

            foreach (var entity in list.Buttons)
            {
                ref var collider = ref entity.Get<Collider>();
                ref var button = ref entity.Get<ButtonComponent>();

                if (collider.ContainsPoint(mousePosition))
                {
                    if (_input.MouseMoved && button.CurrentState == button.NormalState)
                    {
                        if (list.SelectedIndex != -1)
                            ButtonNormal(list.Buttons[list.SelectedIndex]);

                        ChangeButtonState(entity, ref button, button.HoverState);
                        list.SelectedIndex = list.Buttons.IndexOf(entity);
                    }

                    if (_input.MouseCheckPressed(MouseButtons.Left))
                    {
                        ChangeButtonState(entity, ref button, button.PressState);
                    }
                    else if (_input.MouseCheckReleased(MouseButtons.Left) && button.IsDown)
                    {
                        button.OnClick();
                        ChangeButtonState(entity, ref button, button.HoverState);
                    }
                }
                else if (_input.MouseMoved)
                {
                    if (button.CurrentState == button.PressState)
                        ChangeButtonState(entity, ref button, button.HoverState);
                }
            }
        }

        private void ProcessActionInput(float deltaTime, ref ButtonListComponent list)
        {
            if (list.UpAction != -1 && _actions.ActionCheck(list.UpAction))
            {
                if (list.Direction != -1 && list.HoldTimer.State == TimerState.Stopped)
                {
                    list.Direction = -1;
                    ChangeSelected(ref list);

                    list.HoldTimer.Interval = list.InitialWait;
                    list.HoldTimer.Restart();
                }
            }
            else if (list.DownAction != -1 && _actions.ActionCheck(list.DownAction))
            {
                if (list.Direction != 1 || list.HoldTimer.State == TimerState.Stopped)
                {
                    list.Direction = 1;
                    ChangeSelected(ref list);

                    list.HoldTimer.Interval = list.InitialWait;
                    list.HoldTimer.Restart();
                }
            }
            else if (list.Direction != 0)
            {
                list.Direction = 0;
                list.HoldTimer.Stop();
            }

            list.HoldTimer.Update(deltaTime);

            if (list.HoldTimer.Ticked)
            {
                list.HoldTimer.Interval = list.HoldWait;
                ChangeSelected(ref list);
            }

            if (list.SelectedIndex != -1 && list.ClickAction != -1)
            {
                if (_actions.ActionCheckPressed(list.ClickAction))
                {
                    ButtonPress(list.Buttons[list.SelectedIndex]);
                }
                else if (_actions.ActionCheckReleased(list.ClickAction))
                {
                    ref var button = ref list.Buttons[list.SelectedIndex].Get<ButtonComponent>();
                    if (button.IsDown)
                        button.OnClick();
                }
            }
        }

        private void ChangeSelected(ref ButtonListComponent list)
        {
            int oldIndex = list.SelectedIndex;

            list.SelectedIndex += list.Direction;
            if (list.SelectedIndex < 0)
                list.SelectedIndex = list.Buttons.Count - 1;
            else if (list.SelectedIndex >= list.Buttons.Count)
                list.SelectedIndex = 0;

            if (oldIndex != -1)
                ButtonNormal(list.Buttons[oldIndex]);

            ButtonHover(list.Buttons[list.SelectedIndex]);
        }

        private void ChangeButtonState(Entity entity, ref ButtonComponent button, string state)
        {
            if (button.CurrentState == state)
                return;

            ref var player = ref entity.Get<SpriteAnimationPlayer>();
            player.Animation = button.Sprite.Animations[state];

            button.CurrentState = state;

            if (button.CurrentState == button.PressState)
                button.IsDown = true;
            else
                button.IsDown = false;
        }

        private void ButtonNormal(Entity entity)
        {
            ref ButtonComponent button = ref entity.Get<ButtonComponent>();
            ChangeButtonState(entity, ref button, button.NormalState);
        }

        private void ButtonHover(Entity entity)
        {
            ref ButtonComponent button = ref entity.Get<ButtonComponent>();
            ChangeButtonState(entity, ref button, button.HoverState);
        }

        private void ButtonPress(Entity entity)
        {
            ref ButtonComponent button = ref entity.Get<ButtonComponent>();
            ChangeButtonState(entity, ref button, button.PressState);
        }
    }
}
