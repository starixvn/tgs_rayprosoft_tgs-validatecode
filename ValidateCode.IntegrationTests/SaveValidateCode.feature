Feature: Save validate code data to service

@Success
Scenario: Save validate data
	Given token is "4DAF317F-4061-4F00-B48C-398874A57A73"
	And validate code is "123456"
	And playerCode is "aabbcc"
	When I call save data
	Then operation success

@Fail
Scenario: Save validate data conflict
	Given token is "4DAF317F-4061-4F00-B48C-398874A57A73"
	And validate code is "123456"
	And playerCode is "aabbcc"
	When I call save data
	Then operation fail