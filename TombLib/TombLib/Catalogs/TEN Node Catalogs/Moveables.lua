-- !Name "If health of a moveable is..."
-- !Section "Moveable parameters"
-- !Description "Compares selected moveable health with given value."
-- !Conditional "True"
-- !Arguments "NewLine, Moveables, Object to check" "NewLine, CompareOperand, 70, Kind of check"
-- !Arguments "Numerical, 30, Hit points value, [ 0 | 3000 | 0 | 1 | 5 ]" 

LevelFuncs.Engine.Node.TestHitPoints = function(moveableName, operand, value)
	local health = TEN.Objects.GetMoveableByName(moveableName):GetHP()
	return LevelFuncs.Engine.Node.CompareValue(health, value, operand)
end

-- !Name "If ID of a moveable is..."
-- !Section "Moveable parameters"
-- !Description "Checks if moveable belongs to a certain slot ID."
-- !Conditional "True"
-- !Arguments "NewLine, Moveables, Object to check" "NewLine, WadSlots, Object ID to compare to"

LevelFuncs.Engine.Node.TestMoveableId = function(moveableName, objectId)
	return TEN.Objects.GetMoveableByName(moveableName):GetObjectID() == objectId
end

-- !Name "If name of a moveable is..."
-- !Section "Moveable parameters"
-- !Description "Checks if moveable's name is the one specified."
-- !Conditional "True"
-- !Arguments "NewLine, Moveables, Object to check"
-- !Arguments "NewLine, String, Object name to compare to"

LevelFuncs.Engine.Node.TestMoveableName = function(moveableName, name)
	return TEN.Objects.GetMoveableByName(moveableName):GetName() == name
end

-- !Name "If animation of a moveable is..."
-- !Section "Moveable parameters"
-- !Description "Checks if moveable is currently playing specified animation number."
-- !Conditional "True"
-- !Arguments "NewLine, Moveables, 80" "Numerical, 20, [ 0 | 1000 | 0 ], Animation ID"

LevelFuncs.Engine.Node.TestMoveableAnimation = function(moveableName, animationId)
	return TEN.Objects.GetMoveableByName(moveableName):GetAnim() == animationId
end

-- !Name "If state of a moveable is..."
-- !Section "Moveable parameters"
-- !Description "Checks if moveable's current state is the one specified."
-- !Conditional "True"
-- !Arguments "NewLine, Moveables, 80" "Numerical, 20, [ 0 | 1000 | 0 ], State ID"

LevelFuncs.Engine.Node.TestMoveableCurrentState = function(moveableName, stateId)
	return TEN.Objects.GetMoveableByName(moveableName):GetState() == stateId
end

-- !Name "If rotation of a moveable is within range..."
-- !Section "Moveable parameters"
-- !Description "Checks if moveable's current rotation is within specified range."
-- !Conditional "True"
-- !Arguments "NewLine, Moveables, 70"
-- !Arguments "Numerical, 15, [ 0 | 359 | 0 ], In range (in degrees)" "Numerical, 15, [ 0 | 359 | 0 ], Out range (in degrees)"

LevelFuncs.Engine.Node.TestMoveableRotation = function(moveableName, rot1, rot2)
	local rot = TEN.Objects.GetMoveableByName(moveableName):GetRotation().y
	return (rot >= rot1 and rot <= rot2)
end

-- !Name "If position of a moveable is within range..."
-- !Section "Moveable parameters"
-- !Description "Checks if moveable's current position is within specified range."
-- !Description "If single-dimension check is needed, set other dimensions to values well out of level bounds."
-- !Conditional "True"
-- !Arguments "NewLine, Moveables"
-- !Arguments "NewLine, Vector3, [ -1000000 | 1000000 | 0 ], Upper position bound" "NewLine, Vector3, [ -1000000 | 1000000 | 0 ], Lower position bound"

LevelFuncs.Engine.Node.TestMoveablePosition = function(moveableName, pos1, pos2)
	local pos = TEN.Objects.GetMoveableByName(moveableName):GetPosition()
	return (pos.x >= pos1.x and pos.x <= pos2.x and
			pos.y >= pos1.y and pos.y <= pos2.y and
			pos.z >= pos1.z and pos.z <= pos2.z)
end

-- !Name "Enable moveable"
-- !Section "Moveable state"
-- !Description "Enables moveable."
-- !Arguments "NewLine, Moveables"

LevelFuncs.Engine.Node.EnableMoveable = function(moveableName)
    TEN.Objects.GetMoveableByName(moveableName):Enable()
end

-- !Name "Disable moveable"
-- !Section "Moveable state"
-- !Description "Disables moveable."
-- !Arguments "NewLine, Moveables"

LevelFuncs.Engine.Node.DisableMoveable = function(moveableName)
    TEN.Objects.GetMoveableByName(moveableName):Disable()
end

-- !Name "Set moveable's animation"
-- !Section "Moveable parameters"
-- !Description "Sets moveable's animation."
-- !Arguments "NewLine, Moveables, 80" "Numerical, 20, [ 0 | 1000 | 0 ], Animation ID"

LevelFuncs.Engine.Node.SetMoveableAnimation = function(moveableName, animationId)
    TEN.Objects.GetMoveableByName(moveableName):SetAnim(animationId)
end

-- !Name "Set moveable's state"
-- !Section "Moveable parameters"
-- !Description "Sets moveable's next state."
-- !Arguments "NewLine, Moveables, 80" "Numerical, 20, [ 0 | 1000 | 0 ], State ID"

LevelFuncs.Engine.Node.SetMoveableState = function(moveableName, stateId)
    TEN.Objects.GetMoveableByName(moveableName):SetState(stateId)
end

-- !Name "Shatter moveable"
-- !Section "Moveable state"
-- !Description "Shatters object in similar way to shatterable statics."
-- !Arguments "NewLine, Moveables"

LevelFuncs.Engine.Node.ShatterMoveable = function(moveableName)
    TEN.Objects.GetMoveableByName(moveableName):Shatter()
end

-- !Name "Explode moveable"
-- !Section "Moveable state"
-- !Description "Explodes object."
-- !Arguments "NewLine, Moveables"

LevelFuncs.Engine.Node.ExplodeMoveable = function(moveableName)
    TEN.Objects.GetMoveableByName(moveableName):Explode()
end

-- !Name "Play sound near moveable"
-- !Section "Moveable state"
-- !Description "Plays specified sound ID around specified moveable."
-- !Arguments "NewLine, Moveables, Moveable to play sound around" "NewLine, SoundEffects, Sound to play"

LevelFuncs.Engine.Node.PlaySoundAroundMoveable = function(moveableName, soundID)
    TEN.Misc.PlaySound(soundID, TEN.Objects.GetMoveableByName(moveableName):GetPosition())
end

-- !Name "Modify health of a moveable"
-- !Section "Moveable parameters"
-- !Description "Set given moveable health."
-- !Arguments "Enumeration, [ Change | Set ], 25, Change adds/subtracts given value while Set forces it."
-- !Arguments "Numerical, [ -1000 | 1000 | 0 | 1 | 5 ], 15, Health value to define", "NewLine, Moveables"

LevelFuncs.Engine.Node.SetHitPoints = function(operation, value, moveableName)

	if (operation == 0) then
		local moveable = TEN.Objects.GetMoveableByName(moveableName)
		moveable:SetHP(moveable:GetHP() + value)
	else
		TEN.Objects.GetMoveableByName(moveableName):SetHP(value)
	end
end

-- !Name "Modify position of a moveable"
-- !Section "Moveable parameters"
-- !Description "Set given moveable position."
-- !Arguments "NewLine, Enumeration, [ Change | Set ], 25, Change adds/subtracts given value while Set forces it."
-- !Arguments "Vector3, [ -1000000 | 1000000 | 0 | 1 | 32 ], 75, Position value to define"
-- !Arguments "NewLine, Moveables"

LevelFuncs.Engine.Node.SetPosition = function(operation, value, moveableName)

	local moveable = TEN.Objects.GetMoveableByName(moveableName)

	if (operation == 0) then
		local position = moveable:GetPosition();
		position.x = position.x + value.x
		position.y = position.y + value.y
		position.z = position.z + value.z
		moveable:SetPosition(position)
	else
		moveable:SetPosition(value)
	end
end

-- !Name "Modify rotation of a moveable"
-- !Section "Moveable parameters"
-- !Description "Set given moveable rotation."
-- !Arguments "Enumeration, [ Change | Set ], 25, Change adds/subtracts given value while Set forces it."
-- !Arguments "Numerical, [ 0 | 360 | 2 | 1 | 5 ], 15, Rotation value to define", "NewLine, Moveables"

LevelFuncs.Engine.Node.SetRotation = function(operation, value, moveableName)

	local moveable = TEN.Objects.GetMoveableByName(moveableName)
	local rotation = moveable:GetRotation();

	if (operation == 0) then
		local rot = rotation.y + value
		if (rot > 360) then
			rotation.y = rot - 360
		elseif (rot < 0) then
			rotation.y = 360 + rot
		else
			rotation.y = rotation.y + value
		end
	else
		rotation.y = value
	end

	moveable:SetRotation(rotation)
end

-- !Name "Set moveable colour"
-- !Section "Moveable parameters"
-- !Description "Sets moveable tint to a given value."
-- !Arguments "NewLine, Moveables, 80" "Color, 20, Moveable colour" 

LevelFuncs.Engine.Node.SetColor = function(moveableName, color)
    TEN.Objects.GetMoveableByName(moveableName):SetColor(color)
end