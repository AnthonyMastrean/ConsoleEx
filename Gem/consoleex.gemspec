version = File.read(File.expand_path("../VERSION",__FILE__)).strip

Gem::Specification.new do |spec|
  spec.platform    = Gem::Platform::RUBY
  spec.name        = 'consoleex'
  spec.version     = version
  spec.files       = Dir['lib/**/*'] + Dir['docs/**/*']
  spec.summary     = 'A helper that provides supplemental functionality to that available in the System.Console class.'
  spec.description = <<-EOF
    Visual Studio .NET allows you to write console applications, but the built-in class provides limited functionality for controlling the screen and cursor. For more sophisticated access, it is necessary to drop down to the system API calls provided in kernel32.dll.
  EOF
  spec.authors           = 'Tim Sneath'
  spec.email             = 'tims@microsoft.com'
  spec.homepage          = 'http://blogs.msdn.com/b/tims/archive/2003/12/15/57475.aspx'
end