namespace Chickensoft.GodotGame;

using Godot;

//-:cnd:noEmit
#if DEBUG
using System.Reflection;
using Chickensoft.GoDotTest;
#endif
//+:cnd:noEmit

// This entry-point file is responsible for determining if we should run tests.
//
// If you want to edit your game's main entry-point, please see Game.tscn and
// Game.cs instead.

public partial class Main : Node2D {
  public TestEnvironment Environment = default!;

  public override void _Ready() {
//-:cnd:noEmit
#if DEBUG
    // If this is a debug build, use GoDotTest to examine the
    // command line arguments and determine if we should run tests.
    Environment = TestEnvironment.From(OS.GetCmdlineArgs());
    if (Environment.ShouldRunTests) {
      CallDeferred("RunTests");
      return;
    }
#endif
//+:cnd:noEmit

    // If we don't need to run tests, we can just switch to the game scene.
    GetTree().ChangeSceneToFile("res://src/Game.tscn");
  }

  private void RunTests()
    => _ = GoTest.RunTests(Assembly.GetExecutingAssembly(), this, Environment);
}
