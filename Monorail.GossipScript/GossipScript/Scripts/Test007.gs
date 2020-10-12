@GossipScript
{
	# TODO Implement local variables

	def name:$fee			type:int	scope:global	default:200
	def name:$player_money	type:int	scope:global	default:100
}

@p1
{
	print text:"Variable Test"

	call-page node:@p2

	set-var name:$player_money value:$player_money+100

	call-page node:@p2
}

@p2
{
	if expr:$player_money < $fee
	{
		case-true
		{
			print text:"You do not have enough money"
		}
		case-false
		{
			print text:"You have enough money"
		}
	}
}