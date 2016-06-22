

using System;
using System.Collections.Generic;
using System.Linq;

namespace Protogame
{
	public partial class NetworkSynchronisationComponent 
	{
		private void AssignSyncDataToMessage(List<SynchronisedData> dataList, EntityPropertiesMessage message)
		{
			
			var totalString = 0;
			var currentString = 0;
			
			
			var totalInt16 = 0;
			var currentInt16 = 0;
			
			
			var totalInt32 = 0;
			var currentInt32 = 0;
			
			
			var totalSingle = 0;
			var currentSingle = 0;
			
			
			var totalDouble = 0;
			var currentDouble = 0;
			
			
			var totalBoolean = 0;
			var currentBoolean = 0;
			
			
			var totalSingleArray = 0;
			var currentSingleArray = 0;
			
			
			var totalTransform = 0;
			var currentTransform = 0;
			
						
			var typeLookup = new Dictionary<int, int>();
            for (var i = 0; i < dataList.Count; i++)
            {
				if (dataList[i].CurrentValue == null)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeNull;
					continue;
				}

								
				else if (dataList[i].CurrentValue is string)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeString;
					totalString += 1;
				}

								
				else if (dataList[i].CurrentValue is short)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeInt16;
					totalInt16 += 1;
				}

								
				else if (dataList[i].CurrentValue is int)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeInt32;
					totalInt32 += 1;
				}

								
				else if (dataList[i].CurrentValue is float)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeSingle;
					totalSingle += 1;
				}

								
				else if (dataList[i].CurrentValue is double)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeDouble;
					totalDouble += 1;
				}

								
				else if (dataList[i].CurrentValue is bool)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeBoolean;
					totalBoolean += 1;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Vector2)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeVector2;
					totalSingleArray += 2;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Vector3)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeVector3;
					totalSingleArray += 3;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Vector4)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeVector4;
					totalSingleArray += 4;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Quaternion)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeQuaternion;
					totalSingleArray += 4;
				}

								
				else if (dataList[i].CurrentValue is Microsoft.Xna.Framework.Matrix)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeMatrix;
					totalSingleArray += 16;
				}

								
				else if (dataList[i].CurrentValue is Protogame.ITransform)
				{
					typeLookup[i] = EntityPropertiesMessage.PropertyTypeTransform;
					totalTransform += 1;
				}

				
				else
				{
					throw new NotSupportedException("The type " + dataList[i].CurrentValue + " can not be synchronised as a network property.");
				}
			}

						if (totalString > 0)
			{
				message.PropertyValuesString = new string[totalString];
			}
						if (totalInt16 > 0)
			{
				message.PropertyValuesInt16 = new short[totalInt16];
			}
						if (totalInt32 > 0)
			{
				message.PropertyValuesInt32 = new int[totalInt32];
			}
						if (totalSingle > 0)
			{
				message.PropertyValuesSingle = new float[totalSingle];
			}
						if (totalDouble > 0)
			{
				message.PropertyValuesDouble = new double[totalDouble];
			}
						if (totalBoolean > 0)
			{
				message.PropertyValuesBoolean = new bool[totalBoolean];
			}
						if (totalSingleArray > 0)
			{
				message.PropertyValuesSingleArray = new float[totalSingleArray];
			}
						if (totalTransform > 0)
			{
				message.PropertyValuesTransform = new Protogame.NetworkTransform[totalTransform];
			}
			
            for (var ix = 0; ix < dataList.Count; ix++)
            {
				message.PropertyNames[ix] = dataList[ix].Name;
				message.PropertyTypes[ix] = typeLookup[ix];

				switch (typeLookup[ix])
				{
					case EntityPropertiesMessage.PropertyTypeNull:
						// Do nothing.
						break;
									case EntityPropertiesMessage.PropertyTypeString:
					{
											string value = (string)dataList[ix].CurrentValue;
										message.PropertyValuesString[currentString++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeInt16:
					{
											short value = (short)dataList[ix].CurrentValue;
										message.PropertyValuesInt16[currentInt16++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeInt32:
					{
											int value = (int)dataList[ix].CurrentValue;
										message.PropertyValuesInt32[currentInt32++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeSingle:
					{
											float value = (float)dataList[ix].CurrentValue;
										message.PropertyValuesSingle[currentSingle++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeDouble:
					{
											double value = (double)dataList[ix].CurrentValue;
										message.PropertyValuesDouble[currentDouble++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeBoolean:
					{
											bool value = (bool)dataList[ix].CurrentValue;
										message.PropertyValuesBoolean[currentBoolean++] = value;
										}
						break;
									case EntityPropertiesMessage.PropertyTypeVector2:
					{
											var value = ConvertToVector2(dataList[ix].CurrentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeVector3:
					{
											var value = ConvertToVector3(dataList[ix].CurrentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[2];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeVector4:
					{
											var value = ConvertToVector4(dataList[ix].CurrentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[2];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[3];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeQuaternion:
					{
											var value = ConvertToQuaternion(dataList[ix].CurrentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[2];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[3];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeMatrix:
					{
											var value = ConvertToMatrix(dataList[ix].CurrentValue);
											message.PropertyValuesSingleArray[currentSingleArray++] = value[0];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[1];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[2];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[3];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[4];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[5];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[6];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[7];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[8];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[9];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[10];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[11];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[12];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[13];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[14];
											message.PropertyValuesSingleArray[currentSingleArray++] = value[15];
										}
						break;
									case EntityPropertiesMessage.PropertyTypeTransform:
					{
											var value = ConvertToTransform(dataList[ix].CurrentValue);
										message.PropertyValuesTransform[currentTransform++] = value;
										}
						break;
								}
			}
		}
		
		private void AssignMessageToSyncData(EntityPropertiesMessage message, Dictionary<string, SynchronisedData> fullDataList)
		{
			
			var currentString = 0;
			
			
			var currentInt16 = 0;
			
			
			var currentInt32 = 0;
			
			
			var currentSingle = 0;
			
			
			var currentDouble = 0;
			
			
			var currentBoolean = 0;
			
			
			var currentSingleArray = 0;
			
			
			var currentTransform = 0;
			
			
			for (var i = 0; i < message.PropertyNames.Length; i++)
			{
				if (!fullDataList.ContainsKey(message.PropertyNames[i]))
				{
					continue;
				}

				var syncData = fullDataList[message.PropertyNames[i]];

				switch (message.PropertyTypes[i])
				{
					case EntityPropertiesMessage.PropertyTypeNone:
						break;
					case EntityPropertiesMessage.PropertyTypeNull:
						syncData.SetValueDelegate(null);
						break;
										case EntityPropertiesMessage.PropertyTypeString:
					{
											syncData.SetValueDelegate(message.PropertyValuesString[currentString]);
						currentString++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeInt16:
					{
											syncData.SetValueDelegate(message.PropertyValuesInt16[currentInt16]);
						currentInt16++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeInt32:
					{
											syncData.SetValueDelegate(message.PropertyValuesInt32[currentInt32]);
						currentInt32++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeSingle:
					{
											syncData.SetValueDelegate(message.PropertyValuesSingle[currentSingle]);
						currentSingle++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeDouble:
					{
											syncData.SetValueDelegate(message.PropertyValuesDouble[currentDouble]);
						currentDouble++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeBoolean:
					{
											syncData.SetValueDelegate(message.PropertyValuesBoolean[currentBoolean]);
						currentBoolean++;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeVector2:
					{
											syncData.SetValueDelegate(ConvertFromVector2(message.PropertyValuesSingleArray, currentSingleArray));
						currentSingleArray += 2;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeVector3:
					{
											syncData.SetValueDelegate(ConvertFromVector3(message.PropertyValuesSingleArray, currentSingleArray));
						currentSingleArray += 3;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeVector4:
					{
											syncData.SetValueDelegate(ConvertFromVector4(message.PropertyValuesSingleArray, currentSingleArray));
						currentSingleArray += 4;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeQuaternion:
					{
											syncData.SetValueDelegate(ConvertFromQuaternion(message.PropertyValuesSingleArray, currentSingleArray));
						currentSingleArray += 4;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeMatrix:
					{
											syncData.SetValueDelegate(ConvertFromMatrix(message.PropertyValuesSingleArray, currentSingleArray));
						currentSingleArray += 16;
											break;
					}
										case EntityPropertiesMessage.PropertyTypeTransform:
					{
											syncData.SetValueDelegate(ConvertFromTransform(message.PropertyValuesTransform[currentTransform]));
						currentTransform++;
											break;
					}
									}

			}
		}
	}
}

