# Sprites

## Type Definitions

`?` denotes an optional parameter.

### Sprite

_If the Animations property is missing, this object can also include the properties defined in `Region` or `Frame` to create a `Sprite` with a single `Animation` whose name is `Default` with those values. All values, including the Texture can be mixed and matched._

| Field Name | Type | Description |
| --- | --- | --- |
| Texture | `string` | A texture resource used to create the `Sprite`. |
| Width | `int` | Default width of the sprite frames. |
| Height | `int` | Default height of the sprite frames. |
| Thickness | `int` | Default size of the edges for nine patch sprites. |
| FPS | `int` | The number of frames to cycle through each second. Defaults to 0. |
| StartingFrame | `int` | Determines which frame to start on when starting an animation cycle. Defaults to 0. |
| UpdateMode | `SpriteUpdateMode` | A SpriteUpdateMode enum value used to determine how to update the sprite once it's reached the end of one of its cycles. Defaults to SpriteUpdateMode.None |
| Origin | `Origin` | The point in the sprite that corresponds to its draw position. Defaults to (0, 0). |
| UsesRegions | `bool` | Determines whether the animations use a `TextureAtlas` (true) vs a normal Texture (false). Defaults to false. |
| Animations | `List<Animation` | A list of `Animation`s that make up this `Sprite`. |

### Origin

| Field Name | Type | Description |
| --- | --- | --- |
| X | `float` | The X position of the origin in the frame. |
| Y | `float` | The Y position of the origin in the frame. |

### Animation

_Defaults to the value defined in `Sprite` if the property is missing._

_If the Frames property is missing, this object can also include the properties defined in `Region` or `Frame` to add a single frame with those values._

| Field Name | Type | Description |
| --- | --- | --- |
| Name | `string` | The name of the animation in the sprite, such as 'Walking'. If missing, defaults to the index in the `Sprite.Animations` array. |
| UpdateMode | `SpriteUpdateMode` |  SpriteUpdateMode enum value used to determine how to update the sprite once it's reached the end of one of its cycles. |
| Origin | `Origin` | The point in the sprite that corresponds to its draw position. |
| FPS | `int` | he number of frames to cycle through each second. |
| StartingFrame | `int` | Determines which frame to start on when starting an animation cycle. |
| UsesRegions | `bool` | Determines whether the animations use a `TextureAtlas` (true) vs a normal Texture (false). |
| Texture | `string` | A texture resource used to create the `Animation`. |
| Width | `int` | Default width of the sprite frames. |
| Height | `int` | Default height of the sprite frames. |
| Thickness | `int` | Default size of the edges for nine patch sprites. |
| Frames | `List<Frame|Region>` | A list of frames that make up an animation cycle. Can mix `Frame`s and `Region`s. |


### Region

_Defaults to the values defined in `Sprite` or `Animation` if the property is missing, based on the parent of this object._

| Field Name | Type | Description |
| --- | --- | --- |
| Texture | `string` | A texture atlas resource to get the region from. |
| Region | `string` | A region in the texture atlas to use as a frame. |
| Thickness | `int` | If the sprite is a nine patch, determines the size of the edges (currently must be uniform). If the texture region was naturally a nine patch sprite, this is unused. |

### Frame

_Defaults to the values defined in `Sprite` or `Animation` if the property is missing, based on the parent of this object._

| Field Name | Type | Description |
| --- | --- | --- |
| X | `int` | The starting X position (in pixels) in the texture for this frame. |
| Y | `int` | The starting Y position (in pixels) in the texture for this frame. |
| Width | `int` | The width (in pixels) of this frame. |
| Height | `int` | The height (in pixels) of this frame. |
| Thickness | `int` | If the sprite is a nine patch, determines the size of the edges (currently must be uniform). |
| Texture | `string` | A texture resource to create the frame from. |

## Examples

### Sprite With Single Frame

```json
{
	"Texture": "Sprites/MySprite",
	"Width": 32,
	"Height": 32,
	"Origin":
	{
		"X": 16,
		"Y": 16
	},
	"X": 64,
	"Y": 0,
}
```

### Sprite With One Simple and One Complex Animation

```json
{
	"Texture": "Sprites/MySprite",
	"UpdateMode": "Cycle",
	"Animations":
	[
		{
			"Name": "Simple",
			"X": 64,
			"Y": 0,
			"Width": 32,
			"Height": 32
		},
		{
			"Name": "Complex",
			"Texture": "Sprites/MyAnimation"
			"Width": 64,
			"Height": 64,
			"FPS": 7,
			"Frames":
			[
				{
					"X": 0,
					"Y": 0,
				},
				{
					"X": 64,
					"Y": 0
				},
				{
					"X": 0,
					"Y": 64
				},
				{
					"X": 64,
					"Y": 64
				}
			]
		}
	]
}
```