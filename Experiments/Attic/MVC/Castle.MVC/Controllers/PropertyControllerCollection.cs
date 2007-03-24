#region Apache Notice
/*****************************************************************************
 * 
 * Castle.MVC
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 ********************************************************************************/
#endregion

using System.Collections;

namespace Castle.MVC.Controllers
{
	/// <summary>
	/// Description résumée de PairCollection.
	/// </summary>
	public class PropertyControllerCollection: CollectionBase 
	{
		/// <summary>
		/// Constructeur
		/// </summary>
		public PropertyControllerCollection() {}



		/// <summary>
		/// Accès a un élément de la collection par son index
		/// </summary>
		public PropertyController this[int index] 
		{
			get	{ return (PropertyController)List[index]; }
			set { List[index] = value; }
		}

		/// <summary>
		/// Ajoute un(e) Charge
		/// </summary>
		public int Add(PropertyController value) 
		{
			return List.Add(value);
		}

		/// <summary>
		/// Ajoute une liste de Charge à la collection
		/// </summary>
		public void AddRange(PropertyController[] value) 
		{
			for (int i = 0;	i < value.Length; i++) 
			{
				Add(value[i]);
			}
		}

		/// <summary>
		/// Ajoute une liste de Charge à la collection
		/// </summary>
		public void AddRange(PropertyControllerCollection value) 
		{
			for (int i = 0;	i < value.Count; i++) 
			{
				Add(value[i]);
			}
		}

		/// <summary>
		/// Indique si un(e) Charge appartient à la collection
		/// </summary>
		/// <param name="value">Un(e) Charge</param>
		/// <returns>Renvoir vrai s'il/elle appartinet à la collection</returns>
		public bool Contains(PropertyController value) 
		{
			return List.Contains(value);
		}


		/// <summary>
		/// Copie la collection dans un tableau de Charge
		/// </summary>
		/// <param name="array">Un tableau de Charge</param>
		/// <param name="index">Index de début de la copie dans le tableau</param>
		public void CopyTo(PropertyController[] array, int index) 
		{
			List.CopyTo(array, index);
		}

		/// <summary>
		/// Donne la position de l'agent dans la collection.
		/// </summary>
		/// <param name="value">Un(e) Charge</param>
		/// <returns>L'index de l'élément dans la collection.</returns>
		public int IndexOf(PropertyController value) 
		{
			return List.IndexOf(value);
		}
		
		/// <summary>
		/// Insére un(e) Charge dans la collection.
		/// </summary>
		/// <param name="index">L'index d'insertion dans la collection.</param>
		/// <param name="value">Un(e) Charge</param>
		public void Insert(int index, PropertyController value) 
		{
			List.Insert(index, value);
		}
		
		/// <summary>
		/// Enléve un(e) Charge de la collection.
		/// </summary>
		public void Remove(PropertyController value) 
		{
			List.Remove(value);
		}
	}
}

