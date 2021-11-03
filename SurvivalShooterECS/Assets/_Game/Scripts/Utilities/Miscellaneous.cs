using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities {
	public static class Miscellaneous {
		public enum Directions { None = -1, Right, Down, Left, Up }
		public enum Directions8 { None = -1, Right, RightDown, Down, LeftDown, Left, LeftUp, Up, RightUp }
		//Cooodernadas Horizontal e Vertical, 0:y, 1:x
		public static int[, ] coord4 = new int[, ] { { 0, 1 }, {-1, 0 }, { 0, -1 }, { 1, 0 } };
		//Cooodernadas Horizontal e Vertical + Diagonais, 0:y, 1:x
		public static int[, ] coord8 = new int[, ] { { 0, 1 }, {-1, 1 }, {-1, 0 }, {-1, -1 }, { 0, -1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

		//Preenche um Vetor com um valor definido
		public static void Populate<T>(this T[] arr, T value) {
			for (int i = 0; i < arr.Length; i++) {
				arr[i] = value;
			}
		}

		//Preenche uma Matriz com um valor definido
		public static void Populate<T>(this T[, ] arr, T value) {
			for (int j = 0; j < arr.GetLength(0); ++j) {
				for (int i = 0; i < arr.GetLength(1); ++i) {
					arr[j, i] = value;
				}
			}
		}

		//Preenche uma Área da Matriz com um valor definido
		public static void PopulateArea<T>(this T[, ] arr, T value, int x, int y, int xLength, int yLength) {
			for (int j = y; j < y + yLength; ++j) {
				for (int i = x; i < x + xLength; ++i) {
					arr[j, i] = value;
				}
			}
		}

		//Retorna o próximo par, caso seja ímpar
		public static int ToEven(int n) {
			return n % 2 == 0 ? n : n + 1;
		}

		//Retorna o próximo ímpar, caso seja par
		public static int ToOdd(int n) {
			return n % 2 == 0 ? n + 1 : n;
		}

		//Retorna aleatoriamente um dos elementos
		public static T Choose<T>(T[] chances) {
			return chances[Random.Range(0, chances.Length)];
		}

		//StartCoroutine (InvokeRealtimeCoroutine (DoSomething, seconds));
		public static IEnumerator InvokeRealtimeCoroutine(UnityAction action, float seconds) {
			yield return new WaitForSecondsRealtime(seconds);
			if (action != null)
				action();
		}

		public static Vector3 WorldPositionToCanvas(Vector3 position, RectTransform canvas, Camera camera) {
			Vector3 viewport = camera.WorldToViewportPoint(position);
			return new Vector3(viewport.x * canvas.sizeDelta.x, viewport.y * canvas.sizeDelta.y);
		}

		public static Quaternion RotateToDirection(Vector3 position, Vector3 target) {
			Vector3 diff = target - position;
			diff.Normalize();
			float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			return Quaternion.Euler(0f, 0f, rot_z + 90);
		}

		public static void SetCameraOrthographicSizeByWidth(Camera camera, float width) {
			camera.orthographicSize = (width / camera.aspect) / 2;
		}

		public static Vector3 ClampPositionToCameraLimits(Camera camera, Vector3 position, float wOffset = 0, float hOffset = 0) {
			float halfCameraWidth = HalfWidthCamera(camera);
			return new Vector3(Mathf.Clamp(position.x, -(halfCameraWidth - wOffset), halfCameraWidth - wOffset), Mathf.Clamp(position.y, -(camera.orthographicSize - hOffset), camera.orthographicSize - hOffset));
		}

		public static Vector2 MouseToWorldPosition2D(Camera camera, Vector2 mousePosition) {
			return camera.ScreenToWorldPoint((Vector2) mousePosition);
		}

		public static bool CheckScreenBoundaries(Camera camera, Vector3 position, float wOffset = 0, float hOffset = 0) {
			float halfCameraWidth = HalfWidthCamera(camera);
			return !(position.x < -(halfCameraWidth + wOffset)
				|| position.x > halfCameraWidth + wOffset
				|| position.y < -(camera.orthographicSize + hOffset)
				|| position.y > camera.orthographicSize + hOffset);
		}

		public static float HalfWidthCamera(Camera camera) {
			return camera.orthographicSize * camera.aspect;
		}

		public static float CameraViewportWidth(Camera camera, float w) {
			return Mathf.Lerp(-HalfWidthCamera(camera), HalfWidthCamera(camera), w);
		}

		public static float CameraViewportHeight(Camera camera, float h) {
			return Mathf.Lerp(-camera.orthographicSize, camera.orthographicSize, h);
		}

		//Cria uma textura com uma cor
		public static Texture2D MakeTex(int width, int height, Color col) {
			Color[] pix = new Color[width * height];
			for (int i = 0; i < pix.Length; ++i) {
				pix[i] = col;
			}
			Texture2D result = new Texture2D(width, height);
			result.SetPixels(pix);
			result.Apply();
			return result;
		}

		public static Color ColorHEX(string hex) {
			if (hex.Length != 6) return Color.black;

			int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
			return new Color(r / 255f, g / 255f, b / 255f);
		}

		public static float Map(float value, float fromMin, float fromMax, float toMin, float toMax) {
			return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
		}

		public static int MapToInt(float value, float fromMin, float fromMax, int maxInt) {
			int newValue = (int) Mathf.Lerp(0, maxInt, Mathf.InverseLerp(fromMin, fromMax, value));
			return newValue == maxInt ? maxInt - 1 : newValue;
		}

		public static List<T> RandomizeOrder<T>(List<T> list) {
			if (list == null) return null;

			T[] array = list.ToArray();
			for (int i = 0; i < list.Count; ++i) {
				int rand = Random.Range(0, list.Count);
				T temp = array[rand];
				array[rand] = array[i];
				array[i] = temp;
			}

			return new List<T>(array);
		}

		public static int Mod(int x, int m) {
			return (x % m + m) % m;
		}

		public static bool IsDirectoryEmpty(string path) {
			IEnumerable<string> items = Directory.EnumerateFileSystemEntries(path);
			using(IEnumerator<string> en = items.GetEnumerator()) {
				return !en.MoveNext();
			}
		}

		/// <summary>
		/// Get a substring of the first N characters.
		/// </summary>
		public static string Truncate(string source, int length) {
			if (source.Length > length) {
				source = source.Substring(0, length) + "...";
			}
			return source;
		}

		/// <summary>
		/// Return the index of largest float, in the order they were passed.
		/// </summary>
		public static int WhoIsLarger(params float[] others) {
			int id = 0;

			for (int i = 0; i < others.Length; ++i) {
				if (others[i] > others[id])
					id = i;
			}

			return id;
		}
	}
}