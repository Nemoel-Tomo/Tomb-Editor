LevelFuncs.Engine.Node = {}

-- Helper function for value comparisons. Any function which uses
-- CompareOperator arguments should use this helper function for comparison.

LevelFuncs.Engine.Node.CompareValue = function(operand, reference, operator)

	local result = false
	
	-- Fix Lua-specific treatment of bools as non-numerical values
	if (operand == false) then operand = 0 end;
	if (operand == true)  then operand = 1 end;
	if (reference == false) then reference = 0 end;
	if (reference == true)  then reference = 1 end;

	if (operator == 0 and operand == reference) then result = true end
	if (operator == 1 and operand ~= reference) then result = true end
	if (operator == 2 and operand <  reference) then result = true end
	if (operator == 3 and operand <= reference) then result = true end
	if (operator == 4 and operand >  reference) then result = true end
	if (operator == 5 and operand >= reference) then result = true end	
	return result
end

-- Helper function for value modification.

LevelFuncs.Engine.Node.ModifyValue = function(operand, reference, operator)

	local result = reference
	if (operator == 0) then result = reference + operand end
	if (operator == 1) then result = reference - operand end
	if (operator == 2) then result = reference * operand end
	if (operator == 3) then result = reference / operand end
	if (operator == 4) then result = operand end
	return result
end

-- Helper function for easy generation of a display string with all parameters set.

LevelFuncs.Engine.Node.GenerateString = function(text, x, y, center, shadow, color)

	local options = { }
	if (shadow == true) then table.insert(options, TEN.Strings.DisplayStringOption.SHADOW) end
	if (center == true) then table.insert(options, TEN.Strings.DisplayStringOption.CENTER) end
	local rX, rY = TEN.Misc.PercentToScreen(x, y)
	return TEN.Strings.DisplayString(text, rX, rY, color, false, options)
end

LevelFuncs.Engine.Node.SplitString = function(inputStr, delimiter)

   if inputStr == nil then
      inputStr = "%s"
   end
   
   local t = {}
   for str in string.gmatch(inputStr, "([^"..delimiter.."]+)") do
      table.insert(t, str)
   end
   
   return t
end

-- Helper function for converting UI blend mode enum to real enum.

LevelFuncs.Engine.Node.GetBlendMode = function(index)

	local blendID = TEN.Effects.BlendID.OPAQUE

	if index == 0 then
		blendID = TEN.Effects.BlendID.OPAQUE
	elseif index == 1 then
		blendID = TEN.Effects.BlendID.ALPHATEST
	elseif index == 2 then
		blendID = TEN.Effects.BlendID.ADDITIVE
	elseif index == 3 then
		blendID = TEN.Effects.BlendID.NOZTEST
	elseif index == 4 then
		blendID = TEN.Effects.BlendID.SUBTRACTIVE
	elseif index == 5 then
		blendID = TEN.Effects.BlendID.WIREFRAME
	elseif index == 6 then
		blendID = TEN.Effects.BlendID.EXCLUDE
	elseif index == 7 then
		blendID = TEN.Effects.BlendID.SCREEN
	elseif index == 8 then
		blendID = TEN.Effects.BlendID.LIGHTEN
	elseif index == 9 then
		blendID = TEN.Effects.BlendID.ALPHABLEND 
	end

	return blendID
end